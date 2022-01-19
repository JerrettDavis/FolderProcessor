using JetBrains.Annotations;

namespace FolderProcessor.Abstractions.Files;

/// <summary>
/// Represents a file on the filesystem
/// </summary>
[PublicAPI]
public interface IFileRecord
{
    /// <summary>
    /// The unique ID generated to represent instance of the file.
    /// </summary>
    Guid Id { get; }
    
    /// <summary>
    /// The path of the file on the filesystem
    /// </summary>
    string Path { get; init; }

    DateTimeOffset Touched { get; }
    string FileName { get; init; }
}