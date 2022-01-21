using System;
using FluentAssertions;
using FolderProcessor.Models.Files;
using Xunit;

namespace FolderProcessor.UnitTests.Files
{
    public class FileRecordSanityTests
    {
        [Fact]
        public void GuidShouldPersist()
        {
            // Arrange & Act
            var file = new FileRecord(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            var sameFileDiffPath = new FileRecord(file.Id, Guid.NewGuid().ToString(), file.FileName);
        
            // Assert
            file.Id.Should().Be(sameFileDiffPath.Id);
            file.Path.Should().NotBe(sameFileDiffPath.Path);
        }
    }    
}

