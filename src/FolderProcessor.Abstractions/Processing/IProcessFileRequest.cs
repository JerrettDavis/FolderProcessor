using JetBrains.Annotations;
using MediatR;

namespace FolderProcessor.Abstractions.Processing;

[PublicAPI]
public interface IProcessFileRequest : IRequest
{
    Guid FileId { get; set; }
}