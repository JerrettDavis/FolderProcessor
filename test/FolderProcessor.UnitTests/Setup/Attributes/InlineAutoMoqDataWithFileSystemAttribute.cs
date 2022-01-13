using AutoFixture.Xunit2;
using Xunit;

namespace FolderProcessor.UnitTests.Setup.Attributes;

public class InlineAutoMoqDataWithFileSystemAttribute : CompositeDataAttribute
{
    public InlineAutoMoqDataWithFileSystemAttribute(params object[] values)
        : base(new InlineDataAttribute(values), new AutoMoqDataWithFileSystemAttribute())
    {
    }
}