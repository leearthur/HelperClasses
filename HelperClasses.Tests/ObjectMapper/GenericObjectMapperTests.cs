﻿using System;
using System.Linq;
using Xunit;

namespace HelperClasses.Tests.ObjectMapper
{
    public class GenericObjectMapperTests
    {
        #region Map Single Objects

        [Fact]
        public void MapObject_ValidMapExists_ObjectIsReturned()
        {
            // Arrange
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

            // Act 
            var result = target.Map(source);

            // Assert
            HelperMethods.AssertBasicMapping(source, result);
        }

        [Fact]
        public void MapObject_InvalidMapRequest_ExceptionIsThrown()
        {
            // Arrange
            var target = new ObjectMapper<BasicDestinationClass>();
            var source = new BasicSourceClass
            {
                Identifier = 12,
                FullName = "Badger"
            };

            // Act
            var ex = Assert.Throws<InvalidMapRequest<BasicDestinationClass>>(() => target.Map(source));

            // Assert
            Assert.Equal(source.GetType(), ex.SourceType);
            Assert.Equal(typeof(BasicDestinationClass), ex.DestinationType);
            Assert.Same(source, ex.SourceObject);

        }

        [Fact]
        public void MapObject_NullInput_NullIsReturned()
        {
            // Arrange
            var target = new ObjectMapper<BasicDestinationClass>();

            // Act
            var result = target.Map((string)null);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void MapObject_SimpleTypeOutput_ObjectIsMapped()
        {
            // Arrange
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

            // Act 
            var result = target.Map(source);

            // Assert
            Assert.Equal("[Sausage/64]", result);
        }

        #endregion

        #region Map Object Array

        [Fact]
        public void MapArray_MulpleObjects_MultipleMaped()
        {
            // Arrange
            var target = new ObjectMapper<BasicDestinationClass>();
            target.AddMap<BasicSourceClass>(obj =>
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

            // Act
            var result = target.Map(sources).ToArray();

            // Assert
            Assert.Equal(3, result.Length);

            HelperMethods.AssertBasicMapping(sources[0], result[0]);
            HelperMethods.AssertBasicMapping(sources[1], result[1]);
            HelperMethods.AssertBasicMapping(sources[2], result[2]);
        }

        [Fact]
        public void MapArray_NullArrayPassedIn_NullEnumerableReturned()
        {
            // Arrange
            var target = new ObjectMapper<BasicDestinationClass>();
            BasicSourceClass[] sources = null;

            // Act
            var result = target.Map(sources);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.Any());        
        }

        #endregion

        #region Add Map Tests

        [Fact]
        public void AddMap_NullMap_ExceptionThrown()
        {
            // Arrange
            var target = new ObjectMapper<BasicDestinationClass>();

            // Act
            var result = Assert.Throws<ArgumentNullException>(() => target.AddMap<BasicSourceClass>(null));

            // Assert
            Assert.Equal("map", result.ParamName);

        }

        [Fact]
        public void AddMap_DuplicateMap_ExceptionThrown()
        {
            // Arrange
            var target = new ObjectMapper<BasicDestinationClass>();
            BasicDestinationClass map(BasicSourceClass obj)
            {
                return new BasicDestinationClass();
            }

            // Act
            target.AddMap<BasicSourceClass>(map);

            // Assert
            Assert.Throws<ArgumentException>(() => target.AddMap<BasicSourceClass>(map));
        }

        #endregion

        #region Exists Tests

        [Fact]
        public void Exists_TypeExists_ReturnsTrue()
        {
            // Arrange
            var target = new ObjectMapper<BasicDestinationClass>();
            target.AddMap<BasicSourceClass>(obj =>
            {
                return new BasicDestinationClass();
            });

            // Act
            var result = target.Exists(typeof(BasicSourceClass));

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Exists_TypeDoesNotExists_ReturnsFalse()
        {
            // Arrange
            var target = new ObjectMapper<BasicDestinationClass>();

            // Act
            var result = target.Exists(typeof(BasicSourceClass));

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Exists_TypeIsNull_ExceptionThrown()
        {
            // Arrange
            var target = new ObjectMapper<BasicDestinationClass>();

            // Act
            var result = Assert.Throws<ArgumentNullException>(() => target.Exists(null));

            // Assert
            Assert.Equal("sourceType", result.ParamName);
        }

        #endregion        
    }
}
