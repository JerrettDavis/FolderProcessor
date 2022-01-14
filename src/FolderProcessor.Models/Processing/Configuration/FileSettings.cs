using JetBrains.Annotations;

namespace FolderProcessor.Models.Processing.Configuration;

[PublicAPI]
public abstract class FileSettings
{
    /// <summary>
    /// The directory to move the file to
    /// </summary>
    public string Folder { get; set; } = null!;

    /// <summary>
    /// The extension to use for the file
    /// </summary>
    public string FileExtension { get; set; } = null!;

    /// <summary>
    /// If set to true, all files will be set to a unique name. If false, the
    /// original file name will be used. Be aware this is only for the file name,
    /// not its extension. <see cref="FileExtension"/> is used to set a static file
    /// extension for all files 
    /// </summary>
    public bool UseGeneratedName { get; set; }

    /// <summary>
    /// Used to get the name of a file. If <see cref="UseGeneratedName"/> is set
    /// to true, a name will be generated. 
    /// </summary>
    /// <param name="input">The original file name</param>
    /// <returns>The new file name</returns>
    public abstract string GetFileName(string input);
}