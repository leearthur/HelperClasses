using Xunit;

namespace HelperClasses.Tests.ObjectMapper
{
    internal static class HelperMethods
    {
        internal static void AssertBasicMapping(BasicSourceClass source, BasicDestinationClass destination)
        {
            Assert.Equal(source.Identifier, destination.Id);
            Assert.Equal(source.FullName, destination.Name);
        }
    }
}
