using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace RyaUploaderV2.Services
{
    public interface IUploadService
    {
        string UploadMatches();
    }

    public class UploadService : IUploadService
    {
        private static readonly HttpClient Client = new HttpClient();
        
        private readonly ConcurrentBag<string> _lastMatches = new ConcurrentBag<string>();

        private readonly IShareCodeService _shareCodeService;
        private readonly IFileService _fileService;
        private readonly IPathService _pathService;

        public UploadService(IShareCodeService shareCodeService, IFileService fileService, IPathService pathService)
        {
            _shareCodeService = shareCodeService;
            _fileService = fileService;
            _pathService = pathService;
        }
        
        /// <summary>
        /// Uploads the last 8 matches to csgostats.gg
        /// </summary>
        /// <returns>status message</returns>
        public string UploadMatches()
        {
            var matchList = _fileService.ReadMatches(_pathService.GetMatchesPath());
            var newestSharecodes = _shareCodeService.GetNewestDemoUrls(matchList);

            if (newestSharecodes == null) return "Could not get any sharecode from the last 8 demos.";

            Parallel.ForEach(newestSharecodes, async shareCode => { await TryUploadAsync(shareCode); });
            return "All matches have been uploaded";
        }

        /// <summary>
        /// Try to upload a specific match to csgostats.gg
        /// </summary>
        /// <param name="shareCode"></param>
        private async Task TryUploadAsync(string shareCode)
        {
            try
            {
                if (string.IsNullOrEmpty(shareCode) || _lastMatches.Contains(shareCode)) return;

                _lastMatches.Add(shareCode);

                var form = new MultipartFormDataContent { { new StringContent(shareCode), "sharecode" } };

                var response = await Client.PostAsync("https://csgostats.gg/match/upload", form);

                response.EnsureSuccessStatusCode();

                Debug.WriteLine($"Uploaded: {shareCode}");
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Could not upload: {shareCode}");
                Debug.WriteLine(e);
            }
        }

    }
}
