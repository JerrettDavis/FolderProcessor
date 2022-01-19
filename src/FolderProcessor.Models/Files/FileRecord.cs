using FolderProcessor.Abstractions.Files;

namespace FolderProcessor.Models.Files;

public record FileRecord(string Path, string FileName) : IFileRecord
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string FileName { get; init; } = null!;
    public DateTimeOffset Touched { get; } = DateTime.Now;

    public FileRecord(Guid id, string path, string fileName) : 
        this(path, fileName)
    {
        Id = id;
    }

    public FileRecord(IFileRecord fileRecord) :
        this(fileRecord.Id, fileRecord.Path, fileRecord.FileName) {}
}