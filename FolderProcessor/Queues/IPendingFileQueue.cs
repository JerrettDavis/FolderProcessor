namespace FolderProcessor.Queues;

public interface IPendingFileQueue
{
    public Task EnqueueAsync(string fileName, CancellationToken cancellationToken = default);
    public Task DequeueAsync(string fileName, CancellationToken cancellationToken = default);
}