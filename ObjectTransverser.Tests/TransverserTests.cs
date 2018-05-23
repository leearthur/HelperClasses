using System;
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

        #region Simple Objects

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

        [Fact]
        public void Transverse_ClassWithDate_SingleCallback()
        {
            // Arrange
            var target = new Transverser(DefaultPredicate, _callbackStub.Callback);
            var date = DateTime.Now;
            var principle = new ClassWithDate
            {
                Badger = date,
                Sausage = DateTime.Now.AddSeconds(-64)
            };

            // Act
            target.Transverse(principle);

            // Assert
            Assert.Equal(date, _callbackStub.CallbackObjects.Single());
        }

        #endregion

        #region Lists/Arrays

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
        public void Transverse_ClassWithStringList_MultipleCallbacks()
        {
            // Arrange
            var target = new Transverser((obj, prop) => prop.Name == "StringList" && obj.ToString() == "Badger 3", _callbackStub.Callback);
            var principle = new ClassWithStringList
            {
                StringList = new List<string>
                {
                    "Badger 1",
                    "Badger 2",
                    "Badger 3",
                    "Badger 4",
                    "Badger 5"
                }
            };

            // Act
            target.Transverse(principle);

            // Assert
            Assert.Equal(1, _callbackStub.Count);
            Assert.All(_callbackStub.CallbackObjects, o => o.ToString().StartsWith("Badger "));
        }

        #endregion

        #region Dictionary/Hash Tables

        [Fact]
        public void Transverse_ClassWithSingleItemDictionary_SingleCallback()
        {
            // Arrange
            var target = new Transverser(DefaultPredicate, _callbackStub.Callback);
            var principle = new ClassWithDictionary
            {
                DictionaryProperty = new Dictionary<string, ClassWithString>
                {
                    {"Key1", new ClassWithString {Badger = "Inside Badger 1"}},
                    {"Key2", new ClassWithString {Badger = "Badger Value 2"}}
                }
            };

            // Act
            target.Transverse(principle);

            // Assert
            Assert.Equal(2, _callbackStub.Count);
            Assert.Equal("Inside Badger 1", _callbackStub.CallbackObjects.First().ToString());
        }

        [Fact]
        public void Transverse_ClassWithKeyValuePairs_SingleCallback()
        {
            // Arrange
            var target = new Transverser(DefaultPredicate, _callbackStub.Callback);
            var principle = new ClassWithKeyValuePair
            {
                KeyValuePair = new KeyValuePair<string, ClassWithString>("KeyValue_Key1", new ClassWithString { Badger = "Another Inner Badger" })
            };

            // Act
            target.Transverse(principle);

            // Assert
            Assert.Equal("Another Inner Badger", _callbackStub.CallbackObjects.Single().ToString());
        }

        #endregion

        #region Nested Objects

        [Fact]
        public void Transverse_ClassWithNestedObjects_MultipleCallback()
        {
            // Arrange
            var target = new Transverser(DefaultPredicate, _callbackStub.Callback);
            var principle = new ClassWithNestedObjects()
            {
                Badger = "Top Level Badger",
                ObjectList = new List<Foo>
                {
                    CreateFoo("Inner Badger 1", null),
                    CreateFoo(null, "Inner Sausage 1"),
                    CreateFoo("Inner Badger 2", "Inner Sausage 2"),
                    CreateFoo(null, null)
                }
            };

            // Act
            target.Transverse(principle);

            // Assert
            Assert.Equal(3, _callbackStub.Count);
        }

        #endregion

        #region Attribute Tests

        [Fact]
        public void Transverse_SearchForSingleAttribute_SingleCallback()
        {
            // Arrange
            var target = new Transverser(AttributePredicate, _callbackStub.Callback);
            var principle = new ClassWithString
            {
                Badger = "Test Badger",
                Sausage = "Test Sausage"
            };

            // Act
            target.Transverse(principle);

            // Assert
            Assert.Equal("Test Sausage", _callbackStub.CallbackObjects.Single().ToString());
        }

        [Fact]
        public void Transverse_AttributesWithLargeArray_MultipleCallbacks()
        {
            // Arrange
            const int count = 100000;
            var target = new Transverser(AttributePredicate, _callbackStub.Callback);
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

        #endregion

        #region Helper Methods

        private static bool DefaultPredicate(object obj, PropertyInfo prop)
        {
            return prop.Name == "Badger";
        }

        public static bool AttributePredicate(object obj, PropertyInfo prop)
        {
            return prop.CustomAttributes.Any(ca => ca.AttributeType.Name == nameof(TestPropertyAttribute));
        }

        private static Foo CreateFoo(string badger, string sausage)
        {
            return new Foo
            {
                BarProperty = new Bar
                {
                    ObjectList = new ClassWithObjectList
                    {
                        ObjectList = new List<ClassWithString>
                        {
                            new ClassWithString {Badger = badger, Sausage = sausage}
                        }
                    }
                },
                ActionProperty = TestAction,
                FuncProperty = TestFunc
            };
        }

        private static void TestAction(object obj)
        {
        }

        private static bool TestFunc()
        {
            return true;
        }

        #endregion
    }
}
