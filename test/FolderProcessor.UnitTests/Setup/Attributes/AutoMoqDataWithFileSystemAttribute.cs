using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;
using FolderProcessor.UnitTests.Setup.Customizations;

namespace FolderProcessor.UnitTests.Setup.Attributes
{
    public class AutoMoqDataWithFileSystemAttribute: AutoDataAttribute
    {
        public AutoMoqDataWithFileSystemAttribute()
            : base(() => new Fixture()
                .Customize(new AutoMoqCustomization())
                .Customize(new FileSystemCustomization()))
        {
        }
    }    
}

