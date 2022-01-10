using System.Collections.Concurrent;
using FolderProcessor.Models;
using FolderProcessor.Monitoring.Streams;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FolderProcessor.Monitoring;

public class StreamedFolderWatcher : IDisposable
{
    private readonly ConcurrentBag<Func<IAsyncEnumerable<FileRecord>>> _streams;
    private CancellationTokenSource? _cancellationTokenSource;
    private readonly IMediator _mediator;
    private readonly ILogger<StreamedFolderWatcher> _logger;

    public StreamedFolderWatcher(
        IMediator mediator,
        IEnumerable<IFileStream> fileStreams, 
        ILogger<StreamedFolderWatcher> logger)
    {
        _mediator = mediator;
        _logger = logger;
        _streams = new ConcurrentBag<Func<IAsyncEnumerable<FileRecord>>>();
        _cancellationTokenSource = new CancellationTokenSource();

        foreach (var item in fileStreams)
        {
            AddStream(item, _cancellationTokenSource.Token);
        }
    }

    public void AddStream<T>(
        T request, 
        CancellationToken cancellationToken) 
        where T : IStreamRequest<FileRecord>
    {
        _streams.Add(() => _mediator.CreateStream(request, cancellationToken));
        _cancellationTokenSource?.Cancel();
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _cancellationTokenSource = CancellationTokenSource
            .CreateLinkedTokenSource(cancellationToken);

        var streams = _streams.Select(s => s()).ToList();
        while (!cancellationToken.IsCancellationRequested)
        {
            foreach (var stream in _streams.Select(s => s()))
            {
                await foreach (var f in stream.WithCancellation(cancellationToken))
                {
                    _logger.LogInformation("Incoming file {File}", f);
                }
            }
            // await foreach (var file in streams.Merge().WithCancellation(cancellationToken))
            // {
            //     _logger.LogInformation("Incoming file {File}", file);
            // }
            
            await Task.Delay(1000, cancellationToken)
                .ContinueWith(_ => {}, CancellationToken.None);
        }
    }

    public async Task StopAsync()
    {
        _cancellationTokenSource?.Cancel();
    }

    public void Dispose()
    {
        _cancellationTokenSource?.Dispose();
    }
}