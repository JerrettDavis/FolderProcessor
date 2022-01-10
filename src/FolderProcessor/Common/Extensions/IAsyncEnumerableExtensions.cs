using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace FolderProcessor.Common.Extensions;

// ReSharper disable once InconsistentNaming
public static class IAsyncEnumerableExtensions
{
    public static async IAsyncEnumerable<TSource> MergeAsyncEnumerable<TSource>(
        this IList<IAsyncEnumerable<TSource>> sources,
        int debounceTime = 100,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var queue = new ConcurrentQueue<TSource>();
        while (!cancellationToken.IsCancellationRequested)
        {
            var collections = sources
                .Select(s => Task.Run(async () =>
                {
                    await foreach (var file in s.WithCancellation(cancellationToken)) 
                        queue.Enqueue(file);
                }, cancellationToken))
                .ToList();

            while (!Task.WhenAll(collections).IsCompleted)
            {
                while (!queue.IsEmpty)
                    if (queue.TryDequeue(out var record))
                        yield return record;
                
                // Small debounce to prevent an infinite loop from just spinning. 
                await Task.Delay(debounceTime, cancellationToken)
                    .ContinueWith(_ => {}, CancellationToken.None);
            }
        }

        await Task.CompletedTask;
    }
}