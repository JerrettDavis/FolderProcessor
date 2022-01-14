using JetBrains.Annotations;

namespace FolderProcessor.Abstractions.Files;

/// <summary>
/// Represents a file on the filesystem
/// </summary>
[PublicAPI]
public interface IFileRecord
{
    /// <summary>
    /// The path of the file on the filesystem
    /// </summary>
    string Path { get; init; }
}