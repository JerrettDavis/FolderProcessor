using FolderProcessor.Abstractions.Files;
using MediatR;

namespace FolderProcessor.Abstractions.Monitoring.Streams;

public interface IFileStream : IStreamRequest<IFileRecord>
{
    string Folder { get; set; }
}