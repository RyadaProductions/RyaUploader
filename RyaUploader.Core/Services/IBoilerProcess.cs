using System.Threading;
using System.Threading.Tasks;

namespace RyaUploader.Core.Services
{
    public interface IBoilerProcess
    {
        /// <summary>
        /// Start the process to get the latest matches
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>the exit code from boiler</returns>
        Task<int> StartBoilerAsync(CancellationToken cancellationToken);
    }
}
