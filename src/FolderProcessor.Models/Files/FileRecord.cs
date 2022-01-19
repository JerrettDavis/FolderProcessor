using FolderProcessor.Abstractions.Files;

namespace FolderProcessor.Models.Files;

public record FileRecord(string Path) : IFileRecord
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTimeOffset Seen { get; } = DateTime.Now;
}