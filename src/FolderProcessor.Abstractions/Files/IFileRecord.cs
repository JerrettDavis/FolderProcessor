using System;
using JetBrains.Annotations;

namespace FolderProcessor.Abstractions.Files
{
    /// <summary>
    /// Represents a file on the filesystem
    /// </summary>
    [PublicAPI]
    public interface IFileRecord
    {
        /// <summary>
        /// The unique ID generated to represent instance of the file.
        /// </summary>
        Guid Id { get; }
    
        /// <summary>
        /// The path of the file on the filesystem
        /// </summary>
        string Path { get; set; }

        /// <summary>
        /// The last datetime the record touched in some fashion by the internal
        /// mechanisms of the library.
        /// </summary>
        DateTimeOffset Touched { get; }
    
        /// <summary>
        /// The original filename of the file.
        /// </summary>
        string FileName { get; set; }
    }    
}

