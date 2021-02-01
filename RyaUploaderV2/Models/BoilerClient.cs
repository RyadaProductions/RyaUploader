using System;
using System.Threading;
using System.Threading.Tasks;
using RyaUploaderV2.Properties;
using RyaUploaderV2.Services;
using Stylet;

namespace RyaUploaderV2.Models
{
    public class BoilerClient : PropertyChangedBase, IDisposable
    {
        private string _currentState = "Started";
        public string CurrentState
        {
            get => _currentState; 
            set => SetAndNotify(ref _currentState, value);
        }

        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        
        private readonly IUploadService _uploadService;
        private readonly IBoilerProcessService _boilerProcessService;
        private readonly IFileService _fileService;
        private readonly IPathService _pathService;
        private readonly IShareCodeService _shareCodeService;

        private readonly Timer _refreshTimer;

        public BoilerClient(IUploadService uploadService, IBoilerProcessService boilerProcessService, IFileService fileService, IPathService pathService, IShareCodeService shareCodeService)
        {
            _uploadService = uploadService;
            _boilerProcessService = boilerProcessService;
            _fileService = fileService;
            _pathService = pathService;
            _shareCodeService = shareCodeService;

            _refreshTimer = new Timer(async e => { await TimerCallbackAsync(); }, null, 0, 60000);
        }

        private async Task TimerCallbackAsync()
        {
            var result = await _boilerProcessService.StartBoilerAsync(_cts.Token);
            HandleBoilerResult(result);
        }

        /// <summary>
        /// Handle the result by giving an understandable response to the codes that boiler returns
        /// </summary>
        /// <param name="result">The exit code that boiler returned after running</param>
        private void HandleBoilerResult(int result)
        {
            switch (result)
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

                    var matchList = _fileService.ReadMatches(_pathService.MatchFilePath);
                    var newestSharecodes = _shareCodeService.ConvertMatchListToShareCodes(matchList);

                    CurrentState = _uploadService.UploadShareCodes(newestSharecodes) ? "All matches have been uploaded" : "Could not get any sharecode from the last 8 demos.";
                    break;
                default:
                    CurrentState = Resources.UnknownError;
                    break;
            }
        }

        public void Dispose()
        {
            _cts?.Dispose();
            _refreshTimer?.Dispose();
        }
    }
}
