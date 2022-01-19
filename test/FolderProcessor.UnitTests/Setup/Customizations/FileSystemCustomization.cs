using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using AutoFixture;
using FolderProcessor.Abstractions.Stores;
using FolderProcessor.Stores;
using Moq;

namespace FolderProcessor.UnitTests.Setup.Customizations;

public class FileSystemCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        var mock = new MyMockFileSystem();
        fixture.Register<IFileSystem>(() => mock);
        fixture.Register<MockFileSystem>(() => mock);
        fixture.Register(() => mock);
        fixture.Register<ISeenFileStore>(() => new SeenFileStore());
    }
}

public class MyMockFileSystem : MockFileSystem
{
    public override IFileSystemWatcherFactory FileSystemWatcher => FileSystemWatcherFactory;
    public FileSystemWatcherFactoryMock FileSystemWatcherFactory { get; }
    public MyMockFileSystem() : base(null)
    {
        FileSystemWatcherFactory = new FileSystemWatcherFactoryMock(this);
    }
}

public class FileSystemWatcherFactoryMock : IFileSystemWatcherFactory
{
    private Mock<IFileSystemWatcher>? _fileSystemMock;
    private readonly MockFileSystem _parent;

    public FileSystemWatcherFactoryMock(MockFileSystem parent)
    {
        _parent = parent;
    }

    public void NewFile(string path)
    {
        if (Path.HasExtension(path))
            _parent.AddFile(path, new MockFileData(""));
        else 
            _parent.AddDirectory(path);
        
        NewFileEvent(path);
    }

    public void NewFileEvent(string path)
    {
        var dir = Path.GetDirectoryName(path);
        var file = Path.GetFileName(path);
        
        _fileSystemMock?.Raise(f => f.Created += null, _fileSystemMock.Object, 
            new FileSystemEventArgs(WatcherChangeTypes.Created, dir!, file));
    }

    public IFileSystemWatcher CreateNew()
    {
        _fileSystemMock = new Mock<IFileSystemWatcher>();

        return _fileSystemMock.Object;
    }

    public IFileSystemWatcher CreateNew(string path)
    {
        _fileSystemMock = new Mock<IFileSystemWatcher>();

        return _fileSystemMock.Object;
    }

    public IFileSystemWatcher CreateNew(string path, string filter)
    {
        _fileSystemMock = new Mock<IFileSystemWatcher>();

        return _fileSystemMock.Object;
    }
}