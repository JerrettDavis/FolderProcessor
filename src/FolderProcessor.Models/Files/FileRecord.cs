using FolderProcessor.Abstractions.Files;

namespace FolderProcessor.Models.Files;

public record FileRecord(string Path) : IFileRecord
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public DateTimeOffset Seen { get; } = DateTime.Now;

    public FileRecord(Guid id, string path) : this(path)
    {
        Id = id;
    }
}