using System.IO.Abstractions;
using FolderProcessor.Abstractions.Providers;

namespace FolderProcessor.Providers;

public class StaticCompletedDirectoryProvider : 
    StaticDirectoryProvider,
    ICompletedDirectoryProvider
{
    public StaticCompletedDirectoryProvider(
        string folder, 
        IFileSystem fileSystem) : base(folder, fileSystem)
    {
    }
}