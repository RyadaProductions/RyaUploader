using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RyaUploaderV2.Properties;
using RyaUploaderV2.Services;
using RyaUploaderV2.Services.FileServices;
using RyaUploaderV2.Services.ProtobufServices;
using Serilog;
using Stylet;

namespace RyaUploaderV2.Models
{
    public class BoilerClient : PropertyChangedBase, IDisposable
    {
        private string _currentState = "Started";

        /// <summary>
        /// Represents the current state of the uploading process
        /// </summary>
        public string CurrentState
        {
            get => _currentState; 
            set => SetAndNotify(ref _currentState, value);
        }

        public BindableCollection<MatchModel> Matches { get; set; }

        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        
        private readonly IUploader _uploader;
        private readonly IBoilerProcess _boilerProcess;
        private readonly IFileReader _fileReader;
        private readonly IFileWriter _fileWriter;
        private readonly IFilePaths _filePaths;
        private readonly IShareCodeConverter _shareCodeConverter;
        private readonly IProtobufConverter _protobufConverter;

        private readonly Timer _refreshTimer;

        public BoilerClient(
            IUploader uploader, 
            IBoilerProcess boilerProcess, 
            IFileReader fileReader, 
            IFileWriter fileWriter, 
            IFilePaths filePaths, 
            IShareCodeConverter shareCodeConverter, 
            IProtobufConverter protobufConverter)
        {
            _uploader = uploader;
            _boilerProcess = boilerProcess;
            _fileReader = fileReader;
            _fileWriter = fileWriter;
            _filePaths = filePaths;
            _shareCodeConverter = shareCodeConverter;
            _protobufConverter = protobufConverter;

            Matches = new BindableCollection<MatchModel>(_fileReader.ReadMatchesFromJson(_filePaths.JsonMatchesPath));
#if !DEBUG
            _refreshTimer = new Timer(async e => { await RefreshProtobufAsync(); }, null, 0, 60000);
#endif
        }

        /// <summary>
        /// Run boiler.exe and let it download the new protobuf file.
        /// Upload the matches that have not been uploaded previously.
        /// </summary>
        private async Task RefreshProtobufAsync()
        {
            var exitCode = await _boilerProcess.StartBoilerAsync(_cts.Token);
            await HandleBoilerExitCode(exitCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Handle the exitCode by giving an understandable response to the codes that boiler returns
        /// </summary>
        /// <param name="exitCode">The exit code that boiler returned after running</param>
        private async Task HandleBoilerExitCode(int exitCode)
        {
            switch (exitCode)
            {
                case 1:
                    CurrentState = Resources.DialogBoilerNotFound;
                    break;
                case 2:
                    CurrentState = Resources.DialogBoilerIncorrect;
                    break;
                case -1:
                    CurrentState = Resources.DialogInvalidArguments;
                    break;
                case -2:
                    CurrentState = Resources.DialogRestartSteam;
                    break;
                case -3:
                case -4:
                    CurrentState = Resources.DialogSteamNotRunningOrNotLoggedIn;
                    break;
                case -5:
                case -6:
                case -7:
                    CurrentState = Resources.DialogErrorWhileRetrievingMatchesData;
                    break;
                case -8:
                    CurrentState = Resources.DialogNoNewerDemo;
                    break;
                case 0:
                    CurrentState = Resources.BoilerSuccess;
                    await HandleProtobuf().ConfigureAwait(false);
                    break;
                default:
                    CurrentState = Resources.UnknownError;
                    break;
            }
        }

        private async Task HandleProtobuf()
        {
            var protobuf = _fileReader.ReadProtobuf(_filePaths.MatchListPath);
            var matches = _protobufConverter.ProtobufToMatches(protobuf)
                .Where(match => Matches.All(x => match.MatchId != x.MatchId))
                .ToList();

            if (matches.Count == 0)
            {
                Log.Information("The protobuf did not contain new matches.");
                CurrentState = Resources.DialogNoNewerDemo;
                return;
            }

            var shareCodes = _shareCodeConverter.ConvertMatchListToShareCodes(matches);

            CurrentState = await _uploader.UploadShareCodes(shareCodes) ? "All matches have been uploaded" : "Could not get any sharecode from the last 8 demos.";

            Matches.AddRange(matches);

            _fileWriter.SaveMatchesToJson(_filePaths.JsonMatchesPath, Matches);
        }

        public void Dispose()
        {
            _cts?.Dispose();
            _refreshTimer?.Dispose();
        }
    }
}
