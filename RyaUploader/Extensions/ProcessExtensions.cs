using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace RyaUploader.Extensions
{
    public static class ProcessExtensions
    {
        /// <summary>
        /// Extension method for the Processclass. This needs to be awaited to function properly.
        /// Intended to be used when running a process and it can take a while for it to close.
        /// </summary>
        /// <param name="process">The process you want to wait on</param>
        /// <param name="cancellationToken">CancellationToken can be used to quit waiting prematurely</param>
        public static Task WaitForExitAsync(this Process process, CancellationToken cancellationToken = default(CancellationToken))
        {
            var tcs = new TaskCompletionSource<object>();
            process.EnableRaisingEvents = true;
            process.Exited += (sender, args) => tcs.TrySetResult(null);
            if (cancellationToken != default(CancellationToken))
                cancellationToken.Register(tcs.SetCanceled);

            return tcs.Task;
        }
    }
}
