using System.Net.Http;
using System.Threading.Tasks;

namespace RyaUploader.Core.Facade
{
    public interface IHttpClient
    {
        Task<string> GetStringAsync(string requestUri);

        Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content);
    }

    public class HttpClientFacade : IHttpClient
    {
        private static readonly HttpClient Client = new HttpClient();

        public Task<string> GetStringAsync(string requestUri) => Client.GetStringAsync(requestUri);

        public Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content) => Client.PostAsync(requestUri, content);
    }
}
