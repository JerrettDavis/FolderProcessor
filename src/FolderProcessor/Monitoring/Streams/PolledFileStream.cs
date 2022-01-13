using FolderProcessor.Abstractions.Monitoring.Streams;
using JetBrains.Annotations;

namespace FolderProcessor.Monitoring.Streams;

[PublicAPI]
public class PolledFileStream : IFileStream
{
    public string Folder { get; set; } = null!;
    public TimeSpan Interval { get; set; } = TimeSpan.FromSeconds(30);
}