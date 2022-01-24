using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using AutoFixture;
using FolderProcessor.Abstractions.Files;
using FolderProcessor.Abstractions.Monitoring.Filters;
using FolderProcessor.Abstractions.Monitoring.Streams;
using FolderProcessor.Models.Files;
using MediatR;
using Moq;

namespace FolderProcessor.UnitTests.Setup.Customizations
{
    public class FileStreamCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Register<IFileStream>(MockFileStream.Create);
            fixture.Register<IRequestHandler<IFileStream>>(MockFileStreamHandler.Create);
            var filter = fixture.Freeze<Mock<IFileFilter>>();

            filter.Setup(f => f.IsValid(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            
            var mediatrMock = fixture.Freeze<Mock<IMediator>>();

            fixture.Register(() => mediatrMock.Object);
        }
    }

    public class MockFileStream : IFileStream
    {
        public string Folder { get; set; }

        private MockFileStream()
        {
            var dir = new MockFileSystem();
            var root = dir.Path.GetPathRoot(dir.AllDirectories.First());
            
            Folder = dir.Path.Combine(root, Guid.NewGuid().ToString());
        }

        public static MockFileStream Create()
        {
            return new MockFileStream();
        }
    }

    public class MockFileStreamHandler : 
        IRequestHandler<IFileStream>
    {
        private static readonly MockFileStreamHandler Global = new MockFileStreamHandler();
        private static readonly ConcurrentDictionary<string, Channel<IFileRecord>> MockHandlers = 
            new ConcurrentDictionary<string, Channel<IFileRecord>>();
        
        public static async Task AddFile(string path, string file)
        {
            var record = new FileRecord(Path.Combine(path, file), file);
            if (MockHandlers.TryGetValue(path, out var channel))
                await channel.Writer.WriteAsync(record, CancellationToken.None);
        }

        public Task<Unit> Handle(
            IFileStream request, 
            CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<Unit>();
            cancellationToken.Register(s => 
                ((TaskCompletionSource<Unit>)s).SetResult(Unit.Value), tcs);

            return tcs.Task;
        }

        public static MockFileStreamHandler Create()
        {
            return Global;
        }
    }
}

