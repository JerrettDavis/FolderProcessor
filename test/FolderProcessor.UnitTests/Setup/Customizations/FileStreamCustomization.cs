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

namespace FolderProcessor.UnitTests.Setup.Customizations;

public class FileStreamCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Register<IFileStream>(MockFileStream.Create);
        fixture.Register<IStreamRequestHandler<IFileStream, IFileRecord>>(MockFileStreamHandler.Create);
        var filter = fixture.Freeze<Mock<IFileFilter>>();

        filter.Setup(f => f.IsValid(It.IsAny<string>()))
            .ReturnsAsync(true);
        
        var mediatrMock = fixture.Freeze<Mock<IMediator>>();
        
        mediatrMock.Setup(m => m.CreateStream(It.IsAny<MockFileStream>(), It.IsAny<CancellationToken>()))
            .Callback((IStreamRequest<IFileRecord> s, CancellationToken t) =>
            {
                var c = s as MockFileStream;
                Task.Run(async () =>
                {
                    await Task.Delay(100, t);
                    var tasks = MockFiles.Files
                        .ToList()
                        .Select(e => MockFileStreamHandler.AddFile(c!.Folder, e));
                    await Task.WhenAll(tasks);
                }, t);
            })
            .Returns((IFileStream s, CancellationToken t) => fixture.Freeze<MockFileStreamHandler>().Handle(s, t));
        
        fixture.Register(() => mediatrMock.Object);
    }
}

public struct MockFiles
{
    public static IEnumerable<string> Files => new[]
    {
        "tmp.txt",
        "tmp.xml",
        "tmp.bmp",
        "tmp.xsd"
    };
}

public class MockFileStream : IFileStream
{
    private static ConcurrentDictionary<string, IFileStream> _mockFileStreams = new();
    
    public string Folder { get; set; }

    public MockFileStream()
    {
        var dir = new MockFileSystem();
        var root = dir.Path.GetPathRoot(dir.AllDirectories.First());
        
        Folder = dir.Path.Combine(root, Guid.NewGuid().ToString());
    }

    public static MockFileStream Create()
    {
        var fs = new MockFileStream();
        
        _mockFileStreams.TryAdd(fs.Folder, fs);

        return fs;
    }
}

public class MockFileStreamHandler : 
    IStreamRequestHandler<IFileStream, IFileRecord>
{
    private static MockFileStreamHandler _global = new();
    private static ConcurrentDictionary<string, Channel<IFileRecord>> _mockHandlers = new();
    
    public static async Task AddFile(string path, string file)
    {
        var record = new FileRecord(Path.Combine(path, file));
        if (_mockHandlers.TryGetValue(path, out var channel))
            await channel.Writer.WriteAsync(record, CancellationToken.None);
    }

    public IAsyncEnumerable<IFileRecord> Handle(
        IFileStream request, 
        CancellationToken cancellationToken)
    {
        var channel = _mockHandlers.GetOrAdd(request.Folder, _ => Channel.CreateUnbounded<IFileRecord>());
        cancellationToken.Register(() => channel.Writer.Complete());

        return channel.Reader.ReadAllAsync(CancellationToken.None);
    }

    public static MockFileStreamHandler Create()
    {
        return _global;
    }
}