using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using FolderProcessor.Abstractions.Providers;
using FolderProcessor.Files;
using FolderProcessor.Models.Files;
using FolderProcessor.UnitTests.Setup.Attributes;
using FolderProcessor.UnitTests.Setup.Customizations;
using Moq;
using Xunit;

namespace FolderProcessor.UnitTests.Files;

public class FileMoverTests
{
    [Theory]
    [InlineAutoMoqDataWithFileSystem("")]
    [InlineAutoMoqDataWithFileSystem("Completed")]
    public async Task ShouldMoveFile(
        string directory,
        [Frozen] MyMockFileSystem fileSystem,
        [Frozen] Mock<IDirectoryProvider> provider,
        FileRecord fileRecord,
        FileMover fileMover)
    {
        // Arrange
        await fileSystem.File.WriteAllTextAsync(fileRecord.Path, "");
        // Generally we'd use the filename, but since it's auto generated...
        var path = Path.Combine(directory, fileRecord.Path);
        provider.Setup(p => p.GetFileDestinationPathAsync(
                It.Is<string>(i => i == fileRecord.Path), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(path);

        // Act
        var result = await fileMover.MoveFileAsync(
            fileRecord, provider.Object, CancellationToken.None);

        // Assert
        result.Should().Be(path);
        fileSystem.AllFiles.Should().ContainMatch($"*{path}");
    }
}