using System;
using FolderProcessor.Abstractions.Files;
using JetBrains.Annotations;

namespace FolderProcessor.Models.Files
{
    [PublicAPI]
    public class FileRecord : IFileRecord
    {
    public Guid Id { get; set; } = Guid.NewGuid();
    public string FileName { get; set; }
    public string Path { get; set; }
    public DateTimeOffset Touched { get; } = DateTime.Now;

    public FileRecord(Guid id, string path, string fileName) : 
        this(path, fileName)
    {
        Id = id;
    }

    public FileRecord(string path, string fileName)
    {
        Path = path;
        FileName = fileName;
    }

    public FileRecord(IFileRecord fileRecord) :
        this(fileRecord.Id, fileRecord.Path, fileRecord.FileName) {}
    }    
}

