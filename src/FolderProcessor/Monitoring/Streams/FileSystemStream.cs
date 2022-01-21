using System;
using System.IO;
using System.IO.Abstractions;
using System.Threading;
using System.Threading.Tasks;
using FolderProcessor.Abstractions.Monitoring.Streams;
using FolderProcessor.Abstractions.Stores;
using FolderProcessor.Models.Monitoring.Notifications;
using JetBrains.Annotations;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FolderProcessor.Monitoring.Streams
{
    /// <summary>
    /// Instructs the application to setup a new <see cref="FileSystemStreamHandler"/>
    /// matching the passed parameters.
    /// </summary>
    public class FileSystemStream : IFileStream
    {
        /// <inheritdoc />
        public string Folder { get; set; }
    }

    /// <summary>
    /// Uses the <see cref="FileSystemWatcher"/> to continuously monitor a folder
    /// for new files.
    /// </summary>
    [UsedImplicitly]
    public class FileSystemStreamHandler : 
        IRequestHandler<FileSystemStream>
    {
        private readonly ISeenFileStore _seenFileStore;
        private readonly ILogger<FileSystemStreamHandler> _logger;
        private readonly IFileSystem _fileSystem;
        private readonly IPublisher _publisher;

        public FileSystemStreamHandler(
            ISeenFileStore seenFileStore, 
            ILogger<FileSystemStreamHandler> logger, 
            IFileSystem fileSystem,
            IPublisher publisher)
        {
            _seenFileStore = seenFileStore;
            _logger = logger;
            _fileSystem = fileSystem;
            _publisher = publisher;
        }

        public Task<Unit> Handle(
            FileSystemStream request, 
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Started monitoring {Folder}", request.Folder);
            
            var watcher = SetupWatcher(request.Folder, cancellationToken);
            void ShutdownWatcher(TaskCompletionSource<Unit> source)
            {
                watcher.Dispose();
                source.SetResult(Unit.Value);
            }

            var tcs = new TaskCompletionSource<Unit>();
            cancellationToken.Register(s => 
                ShutdownWatcher((TaskCompletionSource<Unit>)s), tcs);
            
            return tcs.Task;
        }

        private IFileSystemWatcher SetupWatcher(
            string folder,
            CancellationToken cancellationToken)
        {
            var watcher = _fileSystem.FileSystemWatcher.CreateNew(folder);
            watcher.NotifyFilter = NotifyFilters.Attributes
                                    | NotifyFilters.CreationTime
                                    | NotifyFilters.DirectoryName
                                    | NotifyFilters.FileName
                                    | NotifyFilters.LastAccess
                                    | NotifyFilters.LastWrite
                                    | NotifyFilters.Security
                                    | NotifyFilters.Size;
            watcher.EnableRaisingEvents = true;
            watcher.Created += (_, args) => OnWatcherOnChanged(args, cancellationToken);

            return watcher;
        }

        private async void OnWatcherOnChanged(
            FileSystemEventArgs args,
            CancellationToken cancellationToken)
        {
            var path = args.FullPath;
            if (await _seenFileStore
                    .ContainsPathAsync(path, cancellationToken)
                    .ConfigureAwait(false)) 
                return;
            
            var fileRecord = _seenFileStore.AddFileRecord(path);
            _logger.LogDebug("Saw {Record}", fileRecord);

            try
            {
                // We receive events about directories, but we don't care about them.
                if ((_fileSystem.File.GetAttributes(path) &
                     FileAttributes.Directory) != 0) return;
                
                await _publisher.Publish(
                        new FileSeenNotification {FileInfo = fileRecord},
                        cancellationToken)
                    .ConfigureAwait(false);
            }
            catch (DirectoryNotFoundException)
            {
                await HandlePathNotFound(path, fileRecord.Id, cancellationToken);
            }
            catch (FileNotFoundException)
            {
                await HandlePathNotFound(path, fileRecord.Id, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning(
                    "Watcher shutdown while {File} was being handled", 
                    args.FullPath);
            }
        }

        private async Task HandlePathNotFound(
            string path, 
            Guid id, 
            CancellationToken cancellationToken)
        {
            _logger.LogWarning(
                "File {File} was not found. During a routine pre-check. " +
                "Will not be broadcast", path);
                
            await _seenFileStore
                .RemoveAsync(id, cancellationToken)
                .ConfigureAwait(false);
        }
    }    
}
