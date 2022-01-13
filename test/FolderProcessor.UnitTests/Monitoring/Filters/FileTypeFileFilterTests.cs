using System.IO.Abstractions.TestingHelpers;
using System.Threading.Tasks;
using FluentAssertions;
using FolderProcessor.Monitoring.Filters;
using Xunit;

namespace FolderProcessor.UnitTests.Monitoring.Filters;

public class FileTypeFileFilterTests
{
    [Theory]
    [InlineData("data/is/here.xml")]
    [InlineData("data/test.xml")]
    [InlineData("/dat.txt")]
    [InlineData("//dat/dat.txt")]
    [InlineData("dat.txt")]
    public async Task ShouldAllReturnValid(string filePath)
    {
        // Arrange
        var fileSystem = new MockFileSystem();
        var filter = new FileTypeFileFilter(fileSystem, new[] {".xml", ".txt"});

        // Act
        var result = await filter.IsValid(filePath);

        // Assert
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData("data/is/here.xml")]
    [InlineData("data/test.xml")]
    [InlineData("/dat.txt")]
    [InlineData("//dat/dat.txt")]
    [InlineData("dat.txt")]
    public async Task ShouldReturnTrueWithNoFilter(string filePath)
    {
        // Arrange
        var fileSystem = new MockFileSystem();
        var filter = new FileTypeFileFilter(fileSystem);

        // Act
        var result = await filter.IsValid(filePath);

        // Assert
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData("data/is/here.xml")]
    [InlineData("data/test.xml")]
    [InlineData("/")]
    [InlineData("")]
    [InlineData("//dat/dat.txt")]
    [InlineData("dat.txt")]
    public async Task ShouldAllReturnInvalid(string filePath)
    {
        // Arrange
        var fileSystem = new MockFileSystem();
        var filter = new FileTypeFileFilter(fileSystem, new[] {".bmp", ".jpg"});

        // Act
        var result = await filter.IsValid(filePath);

        // Assert
        result.Should().BeFalse();
    }
}