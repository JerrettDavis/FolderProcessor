namespace FolderProcessor.Models.Processing.Configuration;

public class WorkingFileSettings : FileSettings
{
    /// <summary>
    /// Generates a guid if <see cref="FileSettings.UseGeneratedName"/> is set to
    /// true, otherwise it returns the input.
    /// </summary>
    /// <param name="input">The initial file name</param>
    /// <returns>The new file name</returns>
    public override string GetFileName(string input) => 
        UseGeneratedName ? Guid.NewGuid().ToString() : input;
    
}