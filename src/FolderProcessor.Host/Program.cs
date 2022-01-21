using System;
using System.Threading.Tasks;
using FolderProcessor.Extensions.Microsoft.DependencyInjection;
using FolderProcessor.Extensions.Microsoft.DependencyInjection.Files;
using FolderProcessor.Extensions.Microsoft.DependencyInjection.Monitoring;
using FolderProcessor.Extensions.Microsoft.DependencyInjection.Processing;
using FolderProcessor.Hosting.Processors;
using FolderProcessor.Models.Monitoring.Configuration;
using Microsoft.Extensions.Hosting;

namespace FolderProcessor.Hosting
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((h, services) =>
                {
                    services
                        // Automatically sets up FolderProcessor with settings loaded from IConfiguration
                        .AddFolderProcessor(h.Configuration)
                        // Since we setup folder processor above, we can just add ad-hoc watchers
                        .AddFolderWatcher(() => new PollingFolderWatcherSettings
                        {
                            Folder = "Data/Child",
                            Interval = TimeSpan.FromSeconds(30)
                        })
                        .AddFolderWatcher(() => new WatcherSettings("Data/Child"))
                        .AddFileTypeFilter(".txt")
                        .UseStaticWorkingFile("Working")
                        .UseStaticCompletedFile("Completed")
                        .UseStaticErroredFile("Errored")
                        .AddProcessor<LogFileContentProcessor>();
                })
                .Build();

            await host.RunAsync();
        }
    }
}

