using FolderProcessor.Models;
using MediatR;

namespace FolderProcessor.Monitoring.Streams;

public class PolledFileStream : IStreamRequest<FileRecord>, IFileStream
{
    public string Folder { get; set; }
    public TimeSpan Interval { get; set; } 
}