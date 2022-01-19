using FolderProcessor.Abstractions.Files;

namespace FolderProcessor.Models.Files;

public record FileRecord(string Path, string FileName) : IFileRecord
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string FileName { get; init; }
    public DateTimeOffset Seen { get; } = DateTime.Now;

    public FileRecord(Guid id, string path, string fileName) : 
        this(path, fileName)
    {
        Id = id;
    }
}