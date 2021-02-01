using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using RyaUploaderV2.Extensions;
using RyaUploaderV2.Properties;
using RyaUploaderV2.Services;

namespace RyaUploaderV2.Models
{
    public class BoilerClient
    {
        public string CurrentState = "Started";

        public static readonly HttpClient Client = new HttpClient();

        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private readonly ConcurrentBag<string> _lastMatches = new ConcurrentBag<string>();

        private readonly ShareCodeService _shareCodeService;
        private readonly PathService _pathService;

        private readonly Timer _refreshTimer;

        public BoilerClient(ShareCodeService shareCodeService, PathService pathService)
        {
            _shareCodeService = shareCodeService;
            _pathService = pathService;

            _refreshTimer = new Timer(async e => { await TimerCallback(); }, null, 0, 60000);
        }

        private async Task TimerCallback()
        {
            var result = await StartBoiler(_cts.Token);
            CurrentState = HandleBoilerResult(result);
        }

        /// <summary>
        /// Start the process to get the latest matches
        /// </summary>
        /// <param name="ct"></param>
        /// <param name="args"></param>
        /// <returns>the exit code from boiler</returns>
        private async Task<int> StartBoiler(CancellationToken ct, string args = "")
        {
            ct.ThrowIfCancellationRequested();

            var path = Path.Combine(Path.GetTempPath(), "RyaUploader", "tempfile.exe");
            var hash = _pathService.GetSha1Hash(path);
            if (!hash.Equals("80F2C8A1F51118FA450AB9E700645508172B01B8")) return 2;

            //@@@ Not sure if this should remain for success of the list update
            //Process[] currentProcess = Process.GetProcessesByName("csgo");
            //if (currentProcess.Length > 0) currentProcess[0].Kill();

            var boiler = new Process
            {
                StartInfo =
                {
                    FileName = path,
                    Arguments = $"\"{_pathService.GetAppDataPath()}\" {args}",
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            boiler.Start();
            await boiler.WaitForExitAsync(ct);

            return boiler.ExitCode;
        }

        /// <summary>
        /// Handle the result by giving an understandable response to the codes that boiler returns
        /// </summary>
        /// <param name="result"></param>
        /// <param name="isRecentMatches"></param>
        private string HandleBoilerResult(int result, bool isRecentMatches = true)
        {
            switch (result)
            {
                case 1:
                    return Resources.DialogBoilerNotFound;
                case 2:
                    return Resources.DialogBoilerIncorrect;
                case -1:
                    return Resources.DialogInvalidArguments;
                case -2:
                    return Resources.DialogRestartSteam;
                case -3:
                case -4:
                    return Resources.DialogSteamNotRunningOrNotLoggedIn;
                case -5:
                case -6:
                case -7:
                    var msg = isRecentMatches
                        ? string.Format(Resources.DialogErrorWhileRetrievingMatchesData, result)
                        : Resources.DialogErrorWhileRetrievingDemoData;
                    return msg;
                case -8:
                    var demoNotFoundMessage = isRecentMatches
                        ? Resources.DialogNoNewerDemo
                        : Resources.DialogDemoFromShareCodeNotAvailable;
                    return demoNotFoundMessage;
                case 0:
                    CurrentState = Resources.BoilerSuccess;
                    return UploadMatches();
                default:
                    return Resources.UnknownError;
            }
        }

        /// <summary>
        /// Upload the last matches to csgostats.gg
        /// </summary>
        public string UploadMatches()
        {
            if (_shareCodeService.GetShareCodes() == null) return "Could not get any sharecode from the last 8 demos.";

            Parallel.ForEach(_shareCodeService.GetShareCodes(), async shareCode => { await Upload(shareCode); });
            return "All matches have been uploaded";
        }

        /// <summary>
        /// Upload a specific match to csgostats.gg
        /// </summary>
        /// <param name="shareCode"></param>
        public async Task Upload(string shareCode)
        {
            try
            {
                if (_lastMatches.Contains(shareCode))
                {
                    return;
                }

                shareCode = shareCode.Replace(@"%20", @"%");

                var form = new MultipartFormDataContent { { new StringContent(shareCode), "sharecode" } };

                var response = await Client.PostAsync("https://csgostats.gg/match/upload", form);

                response.EnsureSuccessStatusCode();
                await response.Content.ReadAsStringAsync();

                _lastMatches.Add(shareCode);
                Console.WriteLine($"Uploaded: {shareCode}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Could not upload: {shareCode}");
                Console.WriteLine(e);
            }
        }
    }
}
