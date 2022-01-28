using FolderProcessor.Abstractions.Common;
using FolderProcessor.Abstractions.Mediator;
using JetBrains.Annotations;

namespace FolderProcessor.Abstractions.Processing;

/// <summary>
/// A request denoting the ID of a file that needs processing
/// </summary>
[PublicAPI]
public interface IProcessFileRequest : IRequest<IProcessFileResult>
{
    /// <summary>
    /// The ID of the file to process
    /// </summary>
    Guid FileId { get; set; }
}