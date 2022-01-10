using MediatR;

namespace FolderProcessor.Processing;

public class Processor
{
    private readonly IMediator _mediator;

    public Processor(IMediator mediator)
    {
        _mediator = mediator;
    }
}