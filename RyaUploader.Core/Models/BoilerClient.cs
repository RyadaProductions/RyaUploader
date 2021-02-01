using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RyaUploader.Core.Services;
using RyaUploader.Core.Services.Converters;
using RyaUploader.Core.Services.FileServices;
using Serilog;
using Stylet;

namespace RyaUploader.Core.Models
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

        public BindableCollection<Match> Matches { get; set; }

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

            Matches = new BindableCollection<Match>(_fileReader.ReadMatchesFromJson(_filePaths.JsonMatchesPath));

            _refreshTimer = new Timer(async e => { await RefreshProtobufAsync(); }, null, 0, 60000);
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
                    CurrentState = "boiler.exe could not be found.";
                    break;
                case 2:
                    CurrentState = "the version of boiler.exe is incorrect";
                    break;
                case -1:
                    CurrentState = "invalids arguments.";
                    break;
                case -2:
                    CurrentState = "steam is restarting.";
                    break;
                case -3:
                case -4:
                    CurrentState = "steam is not running or logged in.";
                    break;
                case -5:
                case -6:
                case -7:
                    CurrentState = "error occured while retrieving matches data.";
                    break;
                case -8:
                    CurrentState = "no new demo has been found.";
                    break;
                case 0:
                    CurrentState = "succesfully pulled recent matches from CSGO, starting the upload process.";
                    await HandleProtobuf().ConfigureAwait(false);
                    break;
                default:
                    CurrentState = "unknown error.";
                    break;
            }
        }

        private async Task HandleProtobuf()
        {
            var protobuf = _fileReader.ReadProtobuf(_filePaths.MatchListPath);

            var newMatches = protobuf.Matches.Where(match => Matches.All(x => match.Matchid != x.MatchId)).ToList();
            
            if (newMatches.Count == 0)
            {
                Log.Information("The protobuf did not contain new matches.");
                CurrentState = "no new demo has been found.";
                return;
            }

            var models = await _protobufConverter.ProtobufToMatches(newMatches);
               
            var shareCodes = _shareCodeConverter.ConvertMatchListToShareCodes(models);

            CurrentState = await _uploader.UploadShareCodes(shareCodes) ? "All matches have been uploaded" : "Could not get any sharecode from the last 8 demos.";

            Matches.AddRange(models);

            _fileWriter.SaveMatchesToJson(_filePaths.JsonMatchesPath, Matches);
        }

        public void Dispose()
        {
            _cts?.Dispose();
            _refreshTimer?.Dispose();
        }
    }
}
