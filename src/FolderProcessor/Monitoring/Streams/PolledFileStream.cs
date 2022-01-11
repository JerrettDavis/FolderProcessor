namespace FolderProcessor.Monitoring.Streams;

public class PolledFileStream : IFileStream
{
    public string Folder { get; set; } = null!;
    public TimeSpan Interval { get; set; } = TimeSpan.FromSeconds(30);
}