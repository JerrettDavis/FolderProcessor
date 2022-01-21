using System.IO.Abstractions;
using FolderProcessor.Abstractions.Providers;

namespace FolderProcessor.Providers
{
    public class StaticErroredDirectoryProvider :
        StaticDirectoryProvider, IErroredDirectoryProvider
    {
        public StaticErroredDirectoryProvider(
            string folder,
            IFileSystem fileSystem) :
            base(folder, fileSystem)
        { }
    }    
}