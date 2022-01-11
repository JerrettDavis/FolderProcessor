using System.Collections.Concurrent;
using FolderProcessor.Models;
using FolderProcessor.Monitoring.Streams;
using FolderProcessor.Processing.Notifications;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FolderProcessor.Monitoring;

public class StreamedFolderWatcher : IDisposable
{
    private readonly ConcurrentBag<Func<IAsyncEnumerable<FileRecord>>> _streams;
    private CancellationTokenSource? _cancellationTokenSource;
    private readonly IMediator _mediator;
    private readonly ILogger<StreamedFolderWatcher> _logger;
    private readonly IPublisher _publisher;

    public StreamedFolderWatcher(
        IMediator mediator,
        IEnumerable<IFileStream> fileStreams,
        ILogger<StreamedFolderWatcher> logger,
        IPublisher publisher)
    {
        _mediator = mediator;
        _logger = logger;
        _publisher = publisher;
        _streams = new ConcurrentBag<Func<IAsyncEnumerable<FileRecord>>>();
        _cancellationTokenSource = new CancellationTokenSource();

        fileStreams.ToList()
            .ForEach(f => AddStream(f, _cancellationTokenSource.Token));
    }

    // TODO: I need to make this class restart if another stream is added while running.
    private void AddStream<T>(
        T request,
        CancellationToken cancellationToken)
        where T : IStreamRequest<FileRecord>
    {
        _streams.Add(() => _mediator.CreateStream(request, cancellationToken));
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _cancellationTokenSource = CancellationTokenSource
            .CreateLinkedTokenSource(cancellationToken);

        // Streams are lazily created at the beginning of the run.
        var streams = _streams.Select(s => 
                Task.Run(s, _cancellationTokenSource.Token))
            .ToArray();
        var deferredStreams = await Task.WhenAll(streams);
        var merged = AsyncEnumerableEx.Merge(deferredStreams);

        await foreach (var file in merged.WithCancellation(_cancellationTokenSource.Token))
        {
            _logger.LogDebug("Incoming {File}", file);
            await _publisher.Publish(
                new FileNeedsProcessingNotification {File = file}, 
                cancellationToken);
        }
    }

    public Task StopAsync()
    {
        _cancellationTokenSource?.Cancel();

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _cancellationTokenSource?.Dispose();
        GC.SuppressFinalize(this);
    }
}