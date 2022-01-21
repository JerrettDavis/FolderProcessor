using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FolderProcessor.Abstractions;
using FolderProcessor.Abstractions.Monitoring.Streams;
using FolderProcessor.Models.Processing.Notifications;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FolderProcessor.Monitoring
{
/// <summary>
/// Receives a collection of <see cref="IFileStream"/> and uses <see cref="IMediator"/>
/// to setup a handler for each.
/// </summary>
public class StreamedFolderWatcher : 
    IWorkerControl, 
    IDisposable
{
    private readonly ConcurrentBag<Func<Task<Unit>>> _streams;
    private CancellationTokenSource _cancellationTokenSource;
    private readonly IMediator _mediator;
    private readonly ILogger<StreamedFolderWatcher> _logger;

    public StreamedFolderWatcher(
        IMediator mediator,
        IEnumerable<IFileStream> fileStreams,
        ILogger<StreamedFolderWatcher> logger)
    {
        _mediator = mediator;
        _logger = logger;
        _streams = new ConcurrentBag<Func<Task<Unit>>>();
        _cancellationTokenSource = new CancellationTokenSource();

        fileStreams.ToList()
            .ForEach(f => AddStream(f, _cancellationTokenSource.Token));
    }
    
    private void AddStream<T>(
        T request,
        CancellationToken cancellationToken)
        where T : IFileStream
    {
        _streams.Add(() => _mediator.Send(request, cancellationToken));
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
            await Task.WhenAll(_streams.Select(s =>
                    Task.Run(s, _cancellationTokenSource.Token))
                .ToArray());
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

    public Task StopAsync(CancellationToken cancellationToken)
    {
        if (!cancellationToken.IsCancellationRequested)
            return Task.CompletedTask;

        _cancellationTokenSource?.Cancel();
        
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _cancellationTokenSource?.Dispose();
        GC.SuppressFinalize(this);
    }
}    
}

