using System.Threading;
using System.Threading.Tasks;
using FolderProcessor.Abstractions;
using Microsoft.Extensions.Hosting;

namespace FolderProcessor.Extensions.Microsoft.DependencyInjection
{
    public class FolderProcessorHostedService : IHostedService
    {
        private readonly IWorkerControl _worker;
        private readonly bool _waitUntilStarted;
        private Task _workerHandle;
        public FolderProcessorHostedService(
            IWorkerControl worker,
            bool waitUntilStarted)
        {
            _worker = worker;
            _waitUntilStarted = waitUntilStarted;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _workerHandle = _worker.StartAsync(cancellationToken);

            return _workerHandle.IsCompleted || _waitUntilStarted
                ? _workerHandle
                : Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return _worker.StopAsync(cancellationToken);
        }
    }    
}

