using FolderProcessor.Models;
using MediatR;

namespace FolderProcessor.Monitoring.Streams;

public class FileSystemStream : IStreamRequest<FileRecord>, IFileStream
{
    public string Folder { get; set; }
}