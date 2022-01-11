using FolderProcessor.Models;
using MediatR;

namespace FolderProcessor.Monitoring.Streams;

public interface IFileStream : IStreamRequest<FileRecord>
{
    string Folder { get; set; }
}