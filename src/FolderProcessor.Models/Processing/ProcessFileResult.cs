using FolderProcessor.Abstractions.Files;
using FolderProcessor.Abstractions.Processing;

namespace FolderProcessor.Models.Processing;

public class ProcessFileResult : IProcessFileResult
{
    public ProcessFileResult(
        IFileRecord fileRecord, 
        bool isSuccessful = true, 
        ICollection<Exception>? errors = null)
    {
        FileRecord = fileRecord;
        IsSuccessful = isSuccessful;
        Errors = errors ?? new List<Exception>();
    }

    public IFileRecord FileRecord { get; }
    public bool IsSuccessful { get; }
    public ICollection<Exception> Errors { get; }
}