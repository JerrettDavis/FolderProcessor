using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;
using FolderProcessor.UnitTests.Setup.Customizations;

namespace FolderProcessor.UnitTests.Setup.Attributes;

public class AutoMoqDataWithFileStreamHandlersAttribute : AutoDataAttribute
{
    public AutoMoqDataWithFileStreamHandlersAttribute() :
        base(() => new Fixture()
            .Customize(new AutoMoqCustomization())
            .Customize(new FileStreamCustomization()))
    {
    }
}