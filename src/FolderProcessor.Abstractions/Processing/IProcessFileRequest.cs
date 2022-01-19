namespace FolderProcessor.Abstractions.Processing;

public interface IProcessFileRequest
{
    Guid FileId { get; set; }
}