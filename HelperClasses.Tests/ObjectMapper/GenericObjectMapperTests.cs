using Xunit;

namespace HelperClasses.Tests.ObjectMapper
{
    public class GenericObjectMapperTests
    {
        private ObjectMapper<BasicDestinationClass> _target;

        public GenericObjectMapperTests()
        {
            _target = new();
        }

        #region Map Single Objects

        [Fact]
        public void MapObject_ValidMapExists_ObjectIsReturned()
        {
            var target = new ObjectMapper<BasicDestinationClass>();
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

            var result = target.Map(source);

            HelperMethods.AssertBasicMapping(source, result);
        }

        [Fact]
        public void MapObject_InvalidMapRequest_ExceptionIsThrown()
        {
            var source = new BasicSourceClass
            {
                Identifier = 12,
                FullName = "Badger"
            };

            var ex = Assert.Throws<InvalidMapRequest<BasicDestinationClass>>(() => _target.Map(source));

            Assert.Equal(source.GetType(), ex.SourceType);
            Assert.Equal(typeof(BasicDestinationClass), ex.DestinationType);
            Assert.Same(source, ex.SourceObject);

        }

        [Fact]
        public void MapObject_NullInput_NullIsReturned()
        {
            var result = _target.Map((string)null);

            Assert.Null(result);
        }

        [Fact]
        public void MapObject_SimpleTypeOutput_ObjectIsMapped()
        {
            var target = new ObjectMapper<string>();
            target.AddMap<BasicSourceClass>(obj =>
            {
                return $"[{obj.FullName}/{obj.Identifier}]";
            });

            var source = new BasicSourceClass
            {
                Identifier = 64,
                FullName = "Sausage"
            };

            var result = target.Map(source);

            Assert.Equal("[Sausage/64]", result);
        }

        #endregion

        #region Map Object Array

        [Fact]
        public void MapArray_MulpleObjects_MultipleMaped()
        {
            _target.AddMap<BasicSourceClass>(obj =>
            {
                return new BasicDestinationClass
                {
                    Id = obj.Identifier,
                    Name = obj.FullName
                };
            });

            var sources = new[] {
                new BasicSourceClass { Identifier = 10, FullName = "Alpha" },
                new BasicSourceClass { Identifier = 20, FullName = "Beta" },
                new BasicSourceClass { Identifier = 30, FullName = "Delta" },
            };

            var result = _target.Map(sources).ToArray();

            Assert.Equal(3, result.Length);

            HelperMethods.AssertBasicMapping(sources[0], result[0]);
            HelperMethods.AssertBasicMapping(sources[1], result[1]);
            HelperMethods.AssertBasicMapping(sources[2], result[2]);
        }

        [Fact]
        public void MapArray_NullArrayPassedIn_NullEnumerableReturned()
        {
            var sources = default(BasicSourceClass[]);

            var result = _target.Map(sources);

            Assert.NotNull(result);
            Assert.False(result.Any());
        }

        #endregion

        #region Add Map Tests

        [Fact]
        public void AddMap_NullMap_ExceptionThrown()
        {
            var result = Assert.Throws<ArgumentNullException>(() => _target.AddMap<BasicSourceClass>(null));

            Assert.Equal("map", result.ParamName);

        }

        [Fact]
        public void AddMap_DuplicateMap_ExceptionThrown()
        {
            BasicDestinationClass map(BasicSourceClass obj)
            {
                return new BasicDestinationClass();
            }

            _target.AddMap<BasicSourceClass>(map);

            Assert.Throws<ArgumentException>(() => _target.AddMap<BasicSourceClass>(map));
        }

        #endregion

        #region Exists Tests

        [Fact]
        public void Exists_TypeExists_ReturnsTrue()
        {
            _target.AddMap<BasicSourceClass>(obj =>
            {
                return new BasicDestinationClass();
            });

            var result = _target.Exists(typeof(BasicSourceClass));

            Assert.True(result);
        }

        [Fact]
        public void Exists_TypeDoesNotExists_ReturnsFalse()
        {
            var result = _target.Exists(typeof(BasicSourceClass));

            Assert.False(result);
        }

        [Fact]
        public void Exists_TypeIsNull_ExceptionThrown()
        {
            var result = Assert.Throws<ArgumentNullException>(() => _target.Exists(null));

            Assert.Equal("sourceType", result.ParamName);
        }

        #endregion        
    }
}
