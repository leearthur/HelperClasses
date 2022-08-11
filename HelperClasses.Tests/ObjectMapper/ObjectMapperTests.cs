using Xunit;

namespace HelperClasses.Tests.ObjectMapper
{
    public class ObjectMapperTests
    {
        private HelperClasses.ObjectMapper _target;

        public ObjectMapperTests()
        {
            _target = new HelperClasses.ObjectMapper();
        }
        #region Map Single Objects

        [Fact]
        public void MapObject_ValidMapExists_ObjectIsReturned()
        {
            _target.AddMap<BasicSourceClass, BasicDestinationClass>(obj =>
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

            var result = _target.Map<BasicDestinationClass>(source);

            HelperMethods.AssertBasicMapping(source, result);
        }

        [Fact]
        public void MapObject_InvaliSourcedMapRequest_ExceptionIsThrown()
        {
            _target.AddMap<string, BasicDestinationClass>(src =>
                new BasicDestinationClass()
            );

            var source = new BasicSourceClass
            {
                Identifier = 12,
                FullName = "Badger"
            };

            var ex = Assert.Throws<InvalidMapRequest<BasicDestinationClass>>(() => _target.Map<BasicDestinationClass>(source));

            Assert.Equal(source.GetType(), ex.SourceType);
            Assert.Equal(typeof(BasicDestinationClass), ex.DestinationType);
            Assert.Same(source, ex.SourceObject);

        }

        [Fact]
        public void MapObject_InvaliDestinationMapRequest_ExceptionIsThrown()
        {
            var source = new BasicSourceClass();

            var ex = Assert.Throws<InvalidMapRequest<BasicDestinationClass>>(() => _target.Map<BasicDestinationClass>(source));

            Assert.Equal(source.GetType(), ex.SourceType);
            Assert.Equal(typeof(BasicDestinationClass), ex.DestinationType);
            Assert.Same(source, ex.SourceObject);
        }

        [Fact]
        public void MapObject_NullInput_NullIsReturned()
        {
            var result = _target.Map<BasicDestinationClass>(default(string));

            Assert.Null(result);
        }

        [Fact]
        public void MapObject_SimpleTypeOutput_ObjectIsMapped()
        {
            _target.AddMap<BasicSourceClass, string>(obj =>
            {
                return $"[{obj.FullName}/{obj.Identifier}]";
            });

            var source = new BasicSourceClass
            {
                Identifier = 64,
                FullName = "Sausage"
            };

            var result = _target.Map<string>(source);

            Assert.Equal("[Sausage/64]", result);
        }

        #endregion

        #region Map Object Array

        [Fact]
        public void MapArray_MulpleObjects_MultipleMaped()
        {
            _target.AddMap<BasicSourceClass, BasicDestinationClass>(obj =>
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

            var result = _target.Map<BasicDestinationClass>(sources).ToArray();

            Assert.Equal(3, result.Length);

            HelperMethods.AssertBasicMapping(sources[0], result[0]);
            HelperMethods.AssertBasicMapping(sources[1], result[1]);
            HelperMethods.AssertBasicMapping(sources[2], result[2]);
        }

        [Fact]
        public void MapArray_NullArrayPassedIn_NullEnumerableReturned()
        {
            var sources = default(BasicSourceClass[]);

            var result = _target.Map<BasicDestinationClass>(sources);

            Assert.NotNull(result);
            Assert.False(result.Any());
        }

        #endregion

        #region Add Map Tests

        [Fact]
        public void AddMap_DuplicateMap_ExceptionThrown()
        {
            static BasicDestinationClass map(BasicSourceClass obj)
            {
                return new BasicDestinationClass();
            }

            _target.AddMap<BasicSourceClass, BasicDestinationClass>(map);

            Assert.Throws<ArgumentException>(() => _target.AddMap<BasicSourceClass, BasicDestinationClass>(map));
        }

        #endregion

        #region Exists Tests

        [Fact]
        public void Exists_TypeExists_ReturnsTrue()
        {
            _target.AddMap<BasicSourceClass, BasicDestinationClass>(obj =>
                new BasicDestinationClass()
            );

            var result = _target.Exists<BasicDestinationClass>(typeof(BasicSourceClass));

            Assert.True(result);
        }

        [Fact]
        public void Exists_TypeDoesNotExists_ReturnsFalse()
        {
            var result = _target.Exists<BasicDestinationClass>(typeof(BasicSourceClass));

            Assert.False(result);
        }

        [Fact]
        public void Exists_TypeIsNull_ExceptionThrown()
        {
            var result = Assert.Throws<ArgumentNullException>(() => _target.Exists<BasicDestinationClass>(null));

            Assert.Equal("sourceType", result.ParamName);
        }

        #endregion        
    }
}
