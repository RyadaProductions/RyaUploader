using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace RyaUploaderV2.Services
{
    public interface IUploadService
    {
        Task<bool> UploadShareCodes(List<string> shareCodes);
    }

    public class UploadService : IUploadService
    {
        private static readonly HttpClient Client = new HttpClient();
        
        private readonly ConcurrentBag<string> _lastMatches = new ConcurrentBag<string>();
        
        /// <summary>
        /// Uploads the last 8 matches to csgostats.gg
        /// </summary>
        /// <returns>status message</returns>
        public async Task<bool> UploadShareCodes(List<string> shareCodes)
        {
            if (shareCodes == null) return false;
            
            await Task.WhenAll(shareCodes.Select(async shareCode => await TryUploadAsync(shareCode)));

            return true;
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
