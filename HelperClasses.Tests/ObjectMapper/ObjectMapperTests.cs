using Xunit;

namespace PropertyEnumerator.Tests.ObjectMapper
{
    public class ObjectMapperTests
    {
        [Fact]
        public void Map_ValidMapExists_ObjectIsReturned()
        {
            // Arrange
            var target = new HelperClasses.ObjectMapper<BasicDestinationClass>();
            target.AddMap<BasicSourceClass>(obj =>
            {
                return new BasicDestinationClass
                {
                    Id = obj.Identifier,
                    Name = obj.FullName
                };
            });

            var source = new BasicSourceClass
            {
                Identifier = 64,
                FullName = "Sausage"
            };

            // Act 
            var result = target.Map(source);

            // Assert
            Assert.Equal(source.Identifier, result.Id);
            Assert.Equal(source.FullName, result.Name);
        }

        [Fact]
        public void Map_InvalidMapRequest_ExceptionIsThrown()
        {
            // Arrange
            var target = new HelperClasses.ObjectMapper<BasicDestinationClass>();
            var source = new BasicSourceClass
            {
                Identifier = 12,
                FullName = "Badger"
            };

            // Act
            var ex = Assert.Throws<HelperClasses.InvalidMapRequest<BasicDestinationClass>>(() => target.Map(source));

            // Assert
            Assert.Equal(source.GetType(), ex.SourceType);
            Assert.Equal(typeof(BasicDestinationClass), ex.DestinationType);
            Assert.Same(source, ex.SourceObject);

        }
    }
}
