
using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using FolderProcessor.Abstractions.Files;
using FolderProcessor.Abstractions.Mediator;
using FolderProcessor.Abstractions.Processing;
using FolderProcessor.Abstractions.Providers;
using FolderProcessor.Abstractions.Stores;
using FolderProcessor.Models.Files;
using FolderProcessor.Models.Processing;
using FolderProcessor.Processing;
using FolderProcessor.Processing.Behaviors;
using FolderProcessor.UnitTests.Setup.Attributes;
using Moq;
using Xunit;

namespace FolderProcessor.UnitTests.Processing.Behaviors;

public class WorkingFileMovingBehaviorTests
{
    [Theory, AutoMoqDataWithFileSystem]
    public async Task ShouldMoveFileToNewFolder(
        [Frozen] ISeenFileStore seenFileStore,
        [Frozen] IWorkingFileStore workingFileStore,
        [Frozen] Mock<IFileMover> fileMover,
        [Frozen] Mock<IRequestHandler<IProcessFileRequest, IProcessFileResult>> handler, 
        FileRecord record,
        WorkingFileMovingBehavior behavior) 
    {
        // Arrange
        handler.Setup(h => h.Handle(It.IsAny<IProcessFileRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ProcessFileResult(record));
        await seenFileStore.AddAsync(record.Id, record);
        
        var request = new ProcessFileRequest {FileId = record.Id};
        var newName = Guid.NewGuid().ToString();
        
        fileMover.Setup(f => f.MoveFileAsync(
            It.IsAny<IFileRecord>(),
            It.IsAny<IDirectoryProvider>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(newName);

        // Act
        await behavior.Handle(request, CancellationToken.None, 
            () => handler.Object.Handle(request, CancellationToken.None));
        var newFile = await workingFileStore.GetAsync(record.Id, CancellationToken.None);
        
        // Assert
        (await workingFileStore.ContainsAsync(record.Id)).Should().BeTrue();
        newFile.Path.Should().Be(newName);
    }
}