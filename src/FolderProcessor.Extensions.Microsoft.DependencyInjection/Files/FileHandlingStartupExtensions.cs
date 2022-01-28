using System.IO.Abstractions;
using FolderProcessor.Abstractions.Monitoring.Filters;
using FolderProcessor.Abstractions.Processing;
using FolderProcessor.Abstractions.Providers;
using FolderProcessor.Mediator.Processing;
using FolderProcessor.Monitoring.Filters;
using FolderProcessor.Processing;
using FolderProcessor.Processing.Behaviors;
using FolderProcessor.Providers;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace FolderProcessor.Extensions.Microsoft.DependencyInjection.Files;

public static class FileHandlingStartupExtensions
{
    /// <summary>
    /// Adds a global <see cref="FileTypeFileFilter"/> for all watchers.
    /// </summary>
    /// <param name="services">The application's service collection.</param>
    /// <param name="fileExtensions">The extensions (including ".") you'd like to include.</param>
    /// <returns>The application service collection, with the newly added filter.</returns>
    public static IServiceCollection AddFileTypeFilter(
        this IServiceCollection services,
        params string[] fileExtensions) =>
        services.AddTransient<IFileFilter>(s =>
            new FileTypeFileFilter(
                s.GetService<IFileSystem>()!, 
                fileExtensions));
    
    /// <summary>
    /// Adds a global <see cref="FileExistsFilter"/> for all watchers.
    /// </summary>
    /// <param name="services">The application's service collection.</param>
    /// <returns>The application service collection, with the newly added filter.</returns>
    public static IServiceCollection AddFileExistsFilter(
        this IServiceCollection services) =>
        services.AddTransient<IFileFilter, FileExistsFilter>();
    
    /// <summary>
    /// Adds a behavior to move all working files to a specific folder.
    /// </summary>
    /// <param name="services">The application's service collection.</param>
    /// <param name="folder">The folder to move working files to.</param>
    /// <returns>The application service collection, with the new file moving behavior.</returns>
    public static IServiceCollection UseStaticWorkingFile(
        this IServiceCollection services,
        string folder) =>
        services
            .AddTransient<IWorkingDirectoryProvider>(s =>
                new StaticWorkingDirectoryProvider(folder, s.GetService<IFileSystem>()!))
            .AddTransient(
                typeof(IPipelineBehavior<ProcessFileRequestFacade, IProcessFileResult>), 
                typeof(WorkingFileMovingBehavior));
    
    /// <summary>
    /// Adds a behavior to move all completed files to a specific folder.
    /// </summary>
    /// <param name="services">The application's service collection.</param>
    /// <param name="folder">The folder to move completed files to.</param>
    /// <returns>The application service collection, with the new file moving behavior.</returns>
    public static IServiceCollection UseStaticCompletedFile(
        this IServiceCollection services,
        string folder) =>
        services
            .AddTransient<ICompletedDirectoryProvider>(s =>
                new StaticCompletedDirectoryProvider(folder, s.GetService<IFileSystem>()!))
            .AddTransient(
                typeof(IPipelineBehavior<ProcessFileRequestFacade, IProcessFileResult>), 
                typeof(CompletedFileMovingBehavior));

    /// <summary>
    /// Adds a behavior to move all files that encounter an error to a specific folder.
    /// </summary>
    /// <param name="services">The application's service collection.</param>
    /// <param name="folder">The folder to move errored files to.</param>
    /// <returns>The application service collection, with the new file moving behavior.</returns>
    public static IServiceCollection UseStaticErroredFile(
        this IServiceCollection services,
        string folder) =>
        services
            .AddTransient<IErroredDirectoryProvider>(s =>
                new StaticErroredDirectoryProvider(folder, s.GetService<IFileSystem>()!))
            .AddTransient(
                typeof(IPipelineBehavior<ProcessFileRequestFacade, IProcessFileResult>), 
                typeof(ErroredFileMovingBehavior));
}