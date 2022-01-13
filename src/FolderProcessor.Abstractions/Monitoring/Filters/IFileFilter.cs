namespace FolderProcessor.Abstractions.Monitoring.Filters;

public interface IFileFilter
{
    Task<bool> IsValid(string input);
}