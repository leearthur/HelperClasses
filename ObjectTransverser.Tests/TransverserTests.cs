using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;

namespace ObjectTransverser.Tests
{
    public class TransverserTests
    {
        private readonly CallbackStub _callbackStub;

        public TransverserTests()
        {
            _callbackStub = new CallbackStub();
        }

        [Fact]
        public void Transverse_ClassWithString_SingleCallback()
        {
            // Arrange
            var target = new Transverser(DefaultPredicate, _callbackStub.Callback);
            var principle = new ClassWithString
            {
                Badger = "Test Badger",
                Sausage = "Test Sausage"
            };

            // Act
            target.Transverse(principle);

            // Assert
            Assert.Equal("Test Badger", _callbackStub.CallbackObjects.Single().ToString());
        }

        [Fact]
        public void Transverse_ClassWithObjectList_MultipleCallbacks()
        {
            // Arrange
            var target = new Transverser(DefaultPredicate, _callbackStub.Callback);
            var principle = new ClassWithObjectList
            {
                ObjectList = new List<ClassWithString>
                {
                    new ClassWithString { Badger = "Badger 1", Sausage = "Sausage 1" },
                    new ClassWithString { Badger = "Badger 2", Sausage = null }
                }
            };

            // Act
            target.Transverse(principle);

            // Assert
            Assert.Equal(2, _callbackStub.Count);
        }

        [Fact]
        public void Transverse_ClassWithLargeArray_MultipleCallbacks()
        {
            // Arrange
            const int count = 100000;
            var target = new Transverser(DefaultPredicate, _callbackStub.Callback);
            var principle = new ClassWithObjectList
            {
                ObjectList = new List<ClassWithString>()
            };

            for (var i = 0; i < count; i++)
            {
                principle.ObjectList.Add(new ClassWithString { Badger = $"Badge {i}", Sausage = $"Sausage {i}" });
            }

            // Act
            target.Transverse(principle);

            // Assert
            Assert.Equal(count, _callbackStub.Count);
        }

        [Fact]
        public void Transverse_ClassWithStringList_NoCallbacks()
        {
            // Arrange
            var target = new Transverser(DefaultPredicate, _callbackStub.Callback);
            var principle = new ClassWithStringList
            {
                StringList = new List<string>
                {
                    "Badger 1",
                    "Badger 2"
                }
            };

            // Act
            target.Transverse(principle);

            // Assert
            Assert.Equal(0, _callbackStub.Count);
        }

        [Fact]
        public void Transverse_ClassWithPrimitives_NoCallbacks()
        {
            // Arrange
            var target = new Transverser(DefaultPredicate, _callbackStub.Callback);
            var principle = new ClassWithPrimitives
            {
                IntProperty = 9,
                BoolProperty = true,
                CharProperty = 'J',
                DoubleProperty = 64.99
            };

            // Act
            target.Transverse(principle);

            // Assert
            Assert.Equal(0, _callbackStub.Count);
        }

        private bool DefaultPredicate(object obj, PropertyInfo prop)
        {
            return prop.Name == "Badger";
        }
    }
}
