using System.Runtime.CompilerServices;

namespace FolderProcessor.Common.Utilities;

public static class AppTaskExt 
{
    public static async IAsyncEnumerable<object?> Timer(
        TimeSpan interval,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var firstRun = true;
        while (!cancellationToken.IsCancellationRequested)
        {
            if (!firstRun)
                await Task.Delay(interval, cancellationToken)
                    .ContinueWith(_ => { }, CancellationToken.None);

            firstRun = false;

            yield return null;
        }
    }
}