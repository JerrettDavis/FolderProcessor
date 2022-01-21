using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using FolderProcessor.Monitoring.Filters;
using FolderProcessor.UnitTests.Setup.Attributes;
using FolderProcessor.UnitTests.Setup.Customizations;
using Xunit;

namespace FolderProcessor.UnitTests.Monitoring.Filters
{
    public class FileTypeFileFilterTests
    {
        [Theory]
        [InlineAutoMoqDataWithFileSystem("data/is/here.xml")]
        [InlineAutoMoqDataWithFileSystem("data/test.xml")]
        [InlineAutoMoqDataWithFileSystem("/dat.txt")]
        [InlineAutoMoqDataWithFileSystem("//dat/dat.txt")]
        [InlineAutoMoqDataWithFileSystem("dat.txt")]
        public async Task ShouldAllReturnValid(
            string filePath,
            [Frozen] MyMockFileSystem fileSystem)
        {
            // Arrange
            var filter = new FileTypeFileFilter(fileSystem, new[] {".xml", ".txt"});

            // Act
            var result = await filter.IsValid(filePath);

            // Assert
            result.Should().BeTrue();
        }

        [Theory]
        [InlineAutoMoqDataWithFileSystem("data/is/here.xml")]
        [InlineAutoMoqDataWithFileSystem("data/test.xml")]
        [InlineAutoMoqDataWithFileSystem("/dat.txt")]
        [InlineAutoMoqDataWithFileSystem("//dat/dat.txt")]
        [InlineAutoMoqDataWithFileSystem("dat.txt")]
        public async Task ShouldReturnTrueWithNoFilter(
            string filePath,
            [Frozen] MyMockFileSystem fileSystem)
        {
            // Arrange
            var filter = new FileTypeFileFilter(fileSystem);

            // Act
            var result = await filter.IsValid(filePath);

            // Assert
            result.Should().BeTrue();
        }

        [Theory]
        [InlineAutoMoqDataWithFileSystem("data/is/here.xml")]
        [InlineAutoMoqDataWithFileSystem("data/test.xml")]
        [InlineAutoMoqDataWithFileSystem("/dat.txt")]
        [InlineAutoMoqDataWithFileSystem("//dat/dat.txt")]
        [InlineAutoMoqDataWithFileSystem("dat.txt")]
        public async Task ShouldAllReturnInvalid(
            string filePath,
            [Frozen] MyMockFileSystem fileSystem)
        {
            // Arrange
            var filter = new FileTypeFileFilter(fileSystem, new[] {".bmp", ".jpg"});

            // Act
            var result = await filter.IsValid(filePath);

            // Assert
            result.Should().BeFalse();
        }
    }    
}