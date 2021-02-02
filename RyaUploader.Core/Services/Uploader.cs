using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using RyaUploader.Core.Facade;
using Serilog;

namespace RyaUploader.Core.Services
{
    public class Uploader : IUploader
    {
        private readonly IHttpClient _client;
        
        public Uploader(IHttpClient client)
        {
            _client = client;
        }


        public async Task<bool> UploadShareCodes(IEnumerable<string> shareCodes)
        {
            if (shareCodes == null) return false;

            await Task.WhenAll(shareCodes.Select(UploadAsync)).ConfigureAwait(false);

            return true;
        }

        /// <summary>
        /// Try to upload a specific match to csgostats.gg
        /// </summary>
        /// <param name="shareCode"></param>
        private async Task UploadAsync(string shareCode)
        {
            try
            {
                if (string.IsNullOrEmpty(shareCode)) return;

                var form = new MultipartFormDataContent { { new StringContent(shareCode), "sharecode" } };

                var response = await _client.PostAsync("https://csgostats.gg/match/upload", form);

                response.EnsureSuccessStatusCode();

                Log.Information($"Uploaded: {shareCode}.");
            }
            catch (Exception exception)
            {
                Log.Error(exception, $"Could not upload: {shareCode}.");
            }
        }

    }
}
