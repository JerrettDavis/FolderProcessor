using FolderProcessor.Abstractions.Files;

namespace FolderProcessor.Abstractions.Processing;

public interface IProcessFileResult
{
    IFileRecord FileRecord { get; }
    bool IsSuccessful { get; }
    ICollection<Exception> Errors { get; }
}