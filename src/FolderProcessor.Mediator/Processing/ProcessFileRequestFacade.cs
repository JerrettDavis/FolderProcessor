using FolderProcessor.Abstractions.Processing;
using FolderProcessor.Processing;
using MediatR;

namespace FolderProcessor.Mediator.Processing;

public class ProcessFileRequestFacade : 
    ProcessFileRequest, 
    IRequest<IProcessFileResult>
{
    
}