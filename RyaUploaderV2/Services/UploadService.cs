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

    public class UploadService
    {
        private static readonly HttpClient Client = new HttpClient();
        
        private readonly ConcurrentBag<string> _lastMatches = new ConcurrentBag<string>();

        private readonly IShareCodeService _shareCodeService;

        public UploadService(IShareCodeService shareCodeService)
        {
            _shareCodeService = shareCodeService;
        }


        /// <summary>
        /// Uploads the last 8 matches to csgostats.gg
        /// </summary>
        /// <returns>status message</returns>
        public string UploadMatches()
        {
            var newestSharecodes = _shareCodeService.GetNewestShareCodes();

            if (newestSharecodes == null) return "Could not get any sharecode from the last 8 demos.";

            Parallel.ForEach(newestSharecodes, async shareCode => { await TryUploadAsync(shareCode); });
            return "All matches have been uploaded";
        }

        /// <summary>
        /// Try to upload a specific match to csgostats.gg
        /// </summary>
        /// <param name="shareCode"></param>
        public async Task TryUploadAsync(string shareCode)
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
