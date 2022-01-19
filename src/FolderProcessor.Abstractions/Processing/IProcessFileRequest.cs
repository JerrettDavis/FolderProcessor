using MediatR;

namespace FolderProcessor.Abstractions.Processing;

public interface IProcessFileRequest : IRequest
{
    Guid FileId { get; set; }
}