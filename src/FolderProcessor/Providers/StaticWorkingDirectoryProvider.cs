using System.IO.Abstractions;
using FolderProcessor.Abstractions.Providers;

namespace FolderProcessor.Providers
{
    public class StaticWorkingDirectoryProvider : 
        StaticDirectoryProvider,
        IWorkingDirectoryProvider
    {
        public StaticWorkingDirectoryProvider(
            string folder, 
            IFileSystem fileSystem) : base(folder, fileSystem)
        {
        }
    }    
}