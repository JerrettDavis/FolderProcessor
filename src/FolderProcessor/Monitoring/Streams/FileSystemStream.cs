using FolderProcessor.Abstractions.Monitoring.Streams;

namespace FolderProcessor.Monitoring.Streams;

public class FileSystemStream : IFileStream
{
    public string Folder { get; set; } = null!;
}