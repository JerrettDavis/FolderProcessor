using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using FolderProcessor.Abstractions.Processing;
using FolderProcessor.Abstractions.Stores;
using FolderProcessor.Processing;
using FolderProcessor.Processing.PreProcessing;
using FolderProcessor.UnitTests.Setup.Attributes;
using FolderProcessor.UnitTests.Setup.Customizations;
using Xunit;

namespace FolderProcessor.UnitTests.Processing.PreProcessing;

public class WorkingFileMovingBehaviorTests
{
    [Theory, AutoMoqDataWithFileSystem]
    public async Task ShouldMoveFileToNewFolder(
        [Frozen] MyMockFileSystem fileSystem,
        [Frozen] ISeenFileStore seenFileStore,
        [Frozen] IWorkingFileStore workingFileStore,
        WorkingFileMovingBehavior<IProcessFileRequest> behavior) 
    {
        // Arrange
        var initDir = fileSystem.AllDirectories.First();
        // Well that's a frightening location for that helper method
        var file = $"{Guid.NewGuid()}.xml";
        var filePath = Path.Combine(initDir, file);
        fileSystem.FileSystemWatcherFactory.NewFile(filePath);
        var fileRecord = seenFileStore.AddFileRecord(filePath);
        var request = new ProcessFileRequest {FileId = fileRecord.Id};

        // Act
        await behavior.Process(request, CancellationToken.None);
        var fileLocation = await workingFileStore.GetAsync(fileRecord.Id);
        // Assert
        (await workingFileStore.ContainsAsync(request.FileId)).Should().BeTrue();
        (await seenFileStore.ContainsAsync(request.FileId)).Should().BeFalse();
        fileLocation.Path.Should().NotBe(fileRecord.Path);
    }

    [Theory, AutoMoqDataWithFileSystem]
    public async Task ShouldGenerateAUniqueNameForFile(
        [Frozen] MyMockFileSystem fileSystem,
        [Frozen] ISeenFileStore seenFileStore,
        [Frozen] IWorkingFileStore workingFileStore,
        WorkingFileMovingBehavior<IProcessFileRequest> behavior)
    {
        // Arrange
        var initDir = fileSystem.AllDirectories.First();
        var file = $"{Guid.NewGuid()}.xml";
        /* We want to run a normal process of a file once. Then we want to add 
         * another file with the same name at the original location. It should
         * handle this. */
        var process = async () =>
        {
            var filePath = Path.Combine(initDir, file);
            fileSystem.FileSystemWatcherFactory.NewFile(filePath);
            var fileRecord = seenFileStore.AddFileRecord(filePath);
            var request = new ProcessFileRequest {FileId = fileRecord.Id};
            // Do it once
            await behavior.Process(request, CancellationToken.None);

            return fileRecord.Id;
        };
        
        // Act
        var id1 = await process();
        var id2 = await process();
        var file1 = await workingFileStore.GetAsync(id1);
        var file2 = await workingFileStore.GetAsync(id2);

        // Assert
        


    }
}