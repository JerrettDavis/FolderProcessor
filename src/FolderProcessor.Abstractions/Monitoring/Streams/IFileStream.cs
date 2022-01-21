using FolderProcessor.Abstractions.Files;
using JetBrains.Annotations;
using MediatR;

namespace FolderProcessor.Abstractions.Monitoring.Streams
{
    /// <summary>
    /// Used to create new File Streams for the application to consume. 
    /// </summary>
    [PublicAPI]
    public interface IFileStream : IStreamRequest<IFileRecord>
    {
        /// <summary>
        /// The folder to monitor.
        /// </summary>
        string Folder { get; set; }
    }    
}