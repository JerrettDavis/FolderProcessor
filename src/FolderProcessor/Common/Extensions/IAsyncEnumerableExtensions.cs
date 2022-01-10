using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace FolderProcessor.Common.Extensions;

// ReSharper disable once InconsistentNaming
public static class IAsyncEnumerableExtensions
{ 
    public static async IAsyncEnumerable<TSource[]> Zip<TSource>(
        this IEnumerable<IAsyncEnumerable<TSource>> sources,
        [EnumeratorCancellation]CancellationToken cancellationToken = default)
    {
        var enumerators = sources
            .Select(x => x.GetAsyncEnumerator(cancellationToken))
            .ToArray();
        try
        {
            while (true)
            {
                var array = new TSource[enumerators.Length];
                for (var i = 0; i < enumerators.Length; i++)
                {
                    if (!await enumerators[i].MoveNextAsync()) yield break;
                    array[i] = enumerators[i].Current;
                }
                yield return array;
            }
        }
        finally
        {
            foreach (var enumerator in enumerators)
            {
                await enumerator.DisposeAsync();
            }
        }
    }
}