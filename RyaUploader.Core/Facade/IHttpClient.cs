using System.Net.Http;
using System.Threading.Tasks;

namespace RyaUploader.Core.Facade
{
    public interface IHttpClient
    {
        Task<string> GetStringAsync(string requestUri);

        Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content);
    }
}
