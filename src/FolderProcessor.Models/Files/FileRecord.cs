using FolderProcessor.Abstractions.Files;

namespace FolderProcessor.Models.Files;

public record FileRecord(string Path) : IFileRecord;