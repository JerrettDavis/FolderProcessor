using System.Collections.Concurrent;
using FolderProcessor.Abstractions.Files;
using FolderProcessor.Abstractions.Monitoring.Filters;
using FolderProcessor.Abstractions.Monitoring.Streams;
using FolderProcessor.Models.Processing.Notifications;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FolderProcessor.Monitoring;

public class StreamedFolderWatcher : IDisposable
{
    private readonly ConcurrentBag<Func<IAsyncEnumerable<IFileRecord>>> _streams;
    private CancellationTokenSource? _cancellationTokenSource;
    private readonly IMediator _mediator;
    private readonly IPublisher _publisher;
    private readonly IEnumerable<IFileFilter> _filters;
    private readonly ILogger<StreamedFolderWatcher> _logger;

    public StreamedFolderWatcher(
        IMediator mediator,
        IEnumerable<IFileStream> fileStreams,
        ILogger<StreamedFolderWatcher> logger,
        IPublisher publisher, 
        IEnumerable<IFileFilter> filters)
    {
        _mediator = mediator;
        _logger = logger;
        _publisher = publisher;
        _filters = filters;
        _streams = new ConcurrentBag<Func<IAsyncEnumerable<IFileRecord>>>();
        _cancellationTokenSource = new CancellationTokenSource();

        fileStreams.ToList()
            .ForEach(f => AddStream(f, _cancellationTokenSource.Token));
    }

    // TODO: I need to make this class restart if another stream is added while running.
    private void AddStream<T>(
        T request,
        CancellationToken cancellationToken)
        where T : IStreamRequest<IFileRecord>
    {
        _streams.Add(() => _mediator.CreateStream(request, cancellationToken));
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _cancellationTokenSource = CancellationTokenSource
            .CreateLinkedTokenSource(cancellationToken);
        
        
        try
        {
            /* There's two Merge methods in AsyncEnumerableEx. Only the one that
             * accepts an array supports concurrency. We need concurrency. */
            // Streams are lazily created at the beginning of the run.
            var streams = await Task.WhenAll(_streams.Select(s =>
                    Task.Run(s, _cancellationTokenSource.Token))
                .ToArray());
            var merged = AsyncEnumerableEx.Merge(streams)
                .WithCancellation(_cancellationTokenSource.Token);
            await foreach (var file in merged)
            {
                // We can add filters to the watcher to only emit files we care about.
                var satisfiedFilters = await _filters
                    .ToAsyncEnumerable()
                    .AllAwaitAsync(async f => await f.IsValid(file.Path),
                        cancellationToken);
                if (!satisfiedFilters) continue;

                _logger.LogDebug("Incoming {File}", file);
                await _publisher.Publish(
                    new FileNeedsProcessingNotification {File = file},
                    cancellationToken);
            }
        }
        catch (AggregateException ae)
        {
            ae.Handle(x =>
            {
                if (x is OperationCanceledException)
                    _logger.LogInformation("File Stream has been shut down");

                return x is OperationCanceledException;
            });
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Folder watcher has been shut down");   
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