using System.IO.Abstractions;
using FolderProcessor.Extensions.Microsoft.DependencyInjection.Monitoring;
using FolderProcessor.Monitoring;
using FolderProcessor.Stores;
using JetBrains.Annotations;
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
    /// <returns>The application service collection, with the newly added Folder Processor</returns>
    public static IServiceCollection AddFolderProcessor(
        this IServiceCollection services,
        IConfiguration configuration) =>
        services
            .AddSingleton<IFileSystem, FileSystem>()
            .AddSingleton<ISeenFileStore, SeenFileStore>()
            .AddSingleton<StreamedFolderWatcher>()
            .AddFolderWatchers(configuration);
}