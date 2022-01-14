using System.Collections.Concurrent;
using FolderProcessor.Abstractions.Files;
using FolderProcessor.Abstractions.Monitoring.Filters;
using FolderProcessor.Abstractions.Monitoring.Streams;
using FolderProcessor.Models.Processing.Notifications;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FolderProcessor.Monitoring;

/// <summary>
/// Receives a collection of <see cref="IFileStream"/> and uses <see cref="IMediator"/>
/// to setup a <see cref="IStreamRequestHandler{TRequest,TResponse}"/> for each.
/// All the streams are combined and each yielded file publishes a
/// <see cref="FileNeedsProcessingNotification"/>, which can be consumed by processors.
/// </summary>
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
    
    private void AddStream<T>(
        T request,
        CancellationToken cancellationToken)
        where T : IStreamRequest<IFileRecord>
    {
        _streams.Add(() => _mediator.CreateStream(request, cancellationToken));
    }

    /// <summary>
    /// Begins monitoring the handlers for each configured <see cref="IFileStream"/>,
    /// publishing a <see cref="FileNeedsProcessingNotification"/> for each returned file.
    /// </summary>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> used
    /// to cancel <see cref="IFileStream"/> monitoring</param>
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
                    .AllAwaitAsync(async f => 
                            await f.IsValid(file.Path, cancellationToken),
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

    public void Dispose()
    {
        _cancellationTokenSource?.Dispose();
        GC.SuppressFinalize(this);
    }
}