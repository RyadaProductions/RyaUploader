using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using RyaUploaderV2.Extensions;

namespace RyaUploaderV2.Services
{
    public interface IBoilerProcessService
    {
        Task<int> StartBoilerAsync(CancellationToken cancellationToken);
    }

    public class BoilerProcessService : IBoilerProcessService
    {
        private readonly IPathService _pathService;
        private readonly IFileService _fileService;

        public BoilerProcessService(IPathService pathService, IFileService fileService)
        {
            _pathService = pathService;
            _fileService = fileService;
        }

        /// <summary>
        /// Start the process to get the latest matches
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>the exit code from boiler</returns>
        public async Task<int> StartBoilerAsync(CancellationToken cancellationToken)
        {
            if (!_fileService.IsBoilerValid) return 2;

            cancellationToken.ThrowIfCancellationRequested();

            var boiler = new Process
            {
                StartInfo =
                {
                    FileName = _pathService.BoilerPath,
                    Arguments = $"\"{_pathService.MatchFilePath}\"",
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            boiler.Start();
            await boiler.WaitForExitAsync(cancellationToken);

            return boiler.ExitCode;
        }
    }
}
