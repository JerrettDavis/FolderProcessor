using System.IO.Abstractions;
using System.Reflection;
using FolderProcessor.Abstractions.Monitoring.Filters;
using FolderProcessor.Abstractions.Providers;
using FolderProcessor.Abstractions.Stores;
using FolderProcessor.Extensions.Microsoft.DependencyInjection.Monitoring;
using FolderProcessor.Monitoring;
using FolderProcessor.Monitoring.Filters;
using FolderProcessor.Providers;
using FolderProcessor.Stores;
using JetBrains.Annotations;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FolderProcessor.Extensions.Microsoft.DependencyInjection;

/// <summary>
/// Defines a set of extensions used for adding FolderProcessor to a project.
/// </summary>
[PublicAPI]
public static class StartupExtensions
{
    /// <summary>
    /// Adds the necessary services to run FolderProcessor
    /// </summary>
    /// <param name="services">The application's service collection.</param>
    /// <param name="configuration">The applications configuration</param>
    /// <param name="assemblies">The assemblies containing mediatr behaviors and handlers</param>
    /// <returns>The application service collection, with the newly added Folder Processor</returns>
    public static IServiceCollection AddFolderProcessor(
        this IServiceCollection services,
        IConfiguration configuration,
        params Assembly[] assemblies)
    {
        var ass = assemblies
            .Union(new[] {typeof(StreamedFolderWatcher).Assembly})
            .ToArray();
        return services
            .AddMediatR(ass)
            .AddSingleton<IFileSystem, FileSystem>()
            .AddSingleton<ISeenFileStore, SeenFileStore>()
            .AddSingleton<IWorkingFileStore, WorkingFileStore>()
            .AddSingleton<StreamedFolderWatcher>()
            .AddFolderWatchers(configuration);
    }

    /// <summary>
    /// Adds the necessary services to run FolderProcessor
    /// </summary>
    /// <param name="services">The application's service collection.</param>
    /// <param name="configuration">The applications configuration</param>
    /// <returns>The application service collection, with the newly added Folder Processor</returns>
    public static IServiceCollection AddFolderProcessor(
        this IServiceCollection services,
        IConfiguration configuration) => 
        AddFolderProcessor(services, configuration, Array.Empty<Assembly>());

    public static IServiceCollection AddFileTypeFilter(
        this IServiceCollection services,
        params string[] fileExtensions) =>
        services.AddTransient<IFileFilter>(s =>
            new FileTypeFileFilter(
                s.GetService<IFileSystem>()!, 
                fileExtensions));

    public static IServiceCollection UseStaticWorkingFile(
        this IServiceCollection services,
        string folder) =>
        services.AddTransient<IWorkingDirectoryProvider>(s =>
            new StaticWorkingDirectoryProvider(folder, s.GetService<IFileSystem>()!));
}