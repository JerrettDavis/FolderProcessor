using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using FolderProcessor.Monitoring.Filters;
using FolderProcessor.UnitTests.Setup.Attributes;
using FolderProcessor.UnitTests.Setup.Customizations;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace FolderProcessor.UnitTests.Monitoring.Filters
{
    public class FileExistsFilterTests
    {
        [Theory, AutoMoqDataWithFileSystem]
        public async Task ShouldSayFileExists(
            [Frozen] MyMockFileSystem fileSystem,
            Mock<ILogger<FileExistsFilter>> logger)
        {
            // Arrange
            var filter = new FileExistsFilter(fileSystem, logger.Object);
            var path = Path.Combine(fileSystem.AllDirectories.First(), $"{Guid.NewGuid()}.xml");
            fileSystem.FileSystemWatcherFactory.NewFile(path);
        
            // Act
            var result = await filter.IsValid(path);
        
            // Assert
            result.Should().BeTrue();
        }
    }    
}

