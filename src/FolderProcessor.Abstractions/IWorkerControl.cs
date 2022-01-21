using System.Threading;
using System.Threading.Tasks;

namespace FolderProcessor.Abstractions
{
    public interface IWorkerControl
    {
        Task StartAsync(CancellationToken cancellationToken);
        Task StopAsync(CancellationToken cancellationToken);
    }    
}

