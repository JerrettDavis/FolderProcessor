using FolderProcessor.Abstractions.Files;
using FolderProcessor.Abstractions.Mediator;

namespace FolderProcessor.Abstractions.Monitoring.Streams;

public interface IFileStreamHandler<in TStreamType> :
    IStreamRequestHandler<TStreamType, IFileRecord> 
    where TStreamType : IFileStream
{
}