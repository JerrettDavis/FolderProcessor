using FolderProcessor.Abstractions.Processing;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace FolderProcessor.Extensions.Microsoft.DependencyInjection.Processing;

[PublicAPI]
public static class ProcessingStartupExtensions
{
    public static IServiceCollection AddProcessor<TProcessor>(
        this IServiceCollection services) 
        where TProcessor : class, IProcessor =>
        services.AddTransient(typeof(IProcessor), typeof(TProcessor));

    public static IServiceCollection AddProcessor<TProcessor>(
        this IServiceCollection services,
        TProcessor processor)
        where TProcessor : class, IProcessor =>
        services.AddTransient<IProcessor>(_ => processor);

    private static IServiceCollection AddProcessor<TProcessor>(
        this IServiceCollection services,
        Func<IServiceProvider, TProcessor> factory)
        where TProcessor : class, IProcessor =>
        services.AddTransient<IProcessor>(factory);
}