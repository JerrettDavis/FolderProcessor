using JetBrains.Annotations;
using MediatR;

namespace FolderProcessor.Abstractions.Processing;

/// <summary>
/// A request denoting the ID of a file that needs processing
/// </summary>
[PublicAPI]
public interface IProcessFileRequest : IRequest
{
    /// <summary>
    /// The ID of the file to process
    /// </summary>
    Guid FileId { get; set; }
}