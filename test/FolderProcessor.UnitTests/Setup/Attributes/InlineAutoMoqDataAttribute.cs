using AutoFixture.Xunit2;
using JetBrains.Annotations;
using Xunit;

namespace FolderProcessor.UnitTests.Setup.Attributes
{
    [PublicAPI]
    public class InlineAutoMoqDataAttribute : CompositeDataAttribute
    {
        public InlineAutoMoqDataAttribute(params object[] values)
            : base(new InlineDataAttribute(values), new AutoMoqDataAttribute())
        {
        }
    }    
}
