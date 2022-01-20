using FolderProcessor.Abstractions.Processing;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace FolderProcessor.Extensions.Microsoft.DependencyInjection.Processing;

[PublicAPI]
public static class ProcessingStartupExtensions
{
    /// <summary>
    /// Adds a new <see cref="IProcessor"/> to the system.
    /// </summary>
    /// <param name="services">The application's service collection.</param>
    /// <typeparam name="TProcessor">The type of <see cref="IProcessor"/> to add.</typeparam>
    /// <returns>The application's service collection, with the newly added processor</returns>
    public static IServiceCollection AddProcessor<TProcessor>(
        this IServiceCollection services) 
        where TProcessor : class, IProcessor =>
        services.AddTransient(typeof(IProcessor), typeof(TProcessor));

    /// <summary>
    /// Adds a new <see cref="IProcessor"/> to the system.
    /// </summary>
    /// <param name="services">The application's service collection.</param>
    /// <param name="processor">The instance of the processor to add.</param>
    /// <typeparam name="TProcessor">The type of <see cref="IProcessor"/> to add.</typeparam>
    /// <returns></returns>
    public static IServiceCollection AddProcessor<TProcessor>(
        this IServiceCollection services,
        TProcessor processor)
        where TProcessor : class, IProcessor =>
        services.AddTransient<IProcessor>(_ => processor);

    /// <summary>
    /// Adds a new <see cref="IProcessor"/> to the system.
    /// </summary>
    /// <param name="services">The application's service collection.</param>
    /// <param name="factory">The factory method to create </param>
    /// <typeparam name="TProcessor">The type of <see cref="IProcessor"/> to add.</typeparam>
    /// <returns></returns>
    private static IServiceCollection AddProcessor<TProcessor>(
        this IServiceCollection services,
        Func<IServiceProvider, TProcessor> factory)
        where TProcessor : class, IProcessor =>
        services.AddTransient<IProcessor>(factory);
}