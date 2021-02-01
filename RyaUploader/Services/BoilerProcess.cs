﻿using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using RyaUploader.Extensions;
using RyaUploader.Services.FileServices;
using Serilog;

namespace RyaUploader.Services
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

    public class BoilerProcess : IBoilerProcess
    {
        private readonly IFilePaths _filePaths;

        public BoilerProcess(IFilePaths filePaths)
        {
            _filePaths = filePaths;
        }

        public async Task<int> StartBoilerAsync(CancellationToken cancellationToken)
        {
            Log.Information("Starting boiler.exe to download the latest protobuf message.");
            cancellationToken.ThrowIfCancellationRequested();

            var boiler = new Process
            {
                StartInfo =
                {
                    FileName = _filePaths.BoilerPath,
                    Arguments = $"\"{_filePaths.MatchListPath}\"",
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            boiler.Start();
            await boiler.WaitForExitAsync(cancellationToken);
            
            Log.Information($"Boiler closed down with exitcode: {boiler.ExitCode}.");

            return boiler.ExitCode;
        }
    }
}
