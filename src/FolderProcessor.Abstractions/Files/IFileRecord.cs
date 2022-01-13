using JetBrains.Annotations;

namespace FolderProcessor.Abstractions.Files;

[PublicAPI]
public interface IFileRecord
{
    string Path { get; init; }
}