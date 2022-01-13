using FolderProcessor.Abstractions.Files;
using JetBrains.Annotations;
using MediatR;

namespace FolderProcessor.Abstractions.Monitoring.Streams;

[PublicAPI]
public interface IFileStream : IStreamRequest<IFileRecord>
{
    string Folder { get; set; }
}