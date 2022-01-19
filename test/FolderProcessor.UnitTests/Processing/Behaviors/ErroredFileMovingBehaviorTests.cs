using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using FolderProcessor.Abstractions.Files;
using FolderProcessor.Abstractions.Processing;
using FolderProcessor.Abstractions.Providers;
using FolderProcessor.Abstractions.Stores;
using FolderProcessor.Models.Files;
using FolderProcessor.Processing;
using FolderProcessor.Processing.Behaviors;
using FolderProcessor.UnitTests.Setup.Attributes;
using MediatR;
using Moq;
using Xunit;

namespace FolderProcessor.UnitTests.Processing.Behaviors;

public class ErroredFileMovingBehaviorTests
{
    [Theory, AutoMoqDataWithFileSystem]
    public async Task ShouldMoveFileToNewFolder(
        [Frozen] IErroredFileStore erroredFileStore,
        [Frozen] IWorkingFileStore workingFileStore,
        [Frozen] Mock<IFileMover> fileMover,
        FileRecord record,
        ErroredFileMovingBehavior<IProcessFileRequest, Unit> behavior) 
    {
        // Arrange
        await workingFileStore.AddAsync(record.Id, record);
        
        var request = new ProcessFileRequest {FileId = record.Id};
        var newName = Guid.NewGuid().ToString();
        
        fileMover.Setup(f => f.MoveFileAsync(
                It.IsAny<IFileRecord>(),
                It.IsAny<IDirectoryProvider>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(newName);

        // Act
        Func<Task<Unit>> func = async () => await behavior
            .Handle(request, CancellationToken.None, () => throw new Exception());

        await func.Should().ThrowAsync<Exception>();
        
        var newFile = await erroredFileStore.GetAsync(record.Id, CancellationToken.None);
        
        // Assert
        (await erroredFileStore.ContainsAsync(record.Id)).Should().BeTrue();
        newFile.Path.Should().Be(newName);
    }
}