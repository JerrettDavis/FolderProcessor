using AutoFixture.Xunit2;
using Xunit;

namespace FolderProcessor.UnitTests.Setup.Attributes;

public class InlineAutoMoqDataAttribute : CompositeDataAttribute
{
    public InlineAutoMoqDataAttribute(params object[] values)
        : base(new InlineDataAttribute(values), new AutoMoqDataAttribute())
    {
    }
}