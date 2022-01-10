using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace FolderProcessor.Common.Extensions;

// ReSharper disable once InconsistentNaming
public static class IAsyncEnumerableExtensions
{
    public static async IAsyncEnumerable<TSource> MergeAsyncEnumerable<TSource>(
        this IEnumerable<IAsyncEnumerable<TSource>> sources,
        TimeSpan? debounceTime = default,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var queue = new ConcurrentQueue<TSource>();
        var tasks = SetupCollections(sources, queue, cancellationToken);
        
        while (!Task.WhenAll(tasks).IsCompleted)
        {
            while (!queue.IsEmpty)
                if (queue.TryDequeue(out var record))
                    yield return record;
                
            // Small debounce to prevent an infinite loop from just spinning. 
            await WaitIfDebounce(debounceTime, cancellationToken);
        }

        await Task.CompletedTask;
    }

    private static Task WaitIfDebounce(
        TimeSpan? debounceTime,
        CancellationToken cancellationToken)
    {
        return debounceTime.HasValue
            ? Task.Delay(debounceTime.Value, cancellationToken)
                .ContinueWith(_ => { }, CancellationToken.None)
            : Task.CompletedTask;
    }

    private static IList<Task> SetupCollections<TSource>(
        IEnumerable<IAsyncEnumerable<TSource>> sources,
        ConcurrentQueue<TSource> queue,
        CancellationToken cancellationToken)
    {
        return sources
            .Select(s => Task.Run(async () =>
            {
                await foreach (var file in s.WithCancellation(cancellationToken)) 
                    queue.Enqueue(file);
            }, cancellationToken))
            .ToList();
    }
}