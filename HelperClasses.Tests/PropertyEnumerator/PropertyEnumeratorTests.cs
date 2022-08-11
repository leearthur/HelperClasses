using System.Reflection;
using Xunit;

namespace HelperClasses.Tests.PropertyEnumerator
{
    public class PropertyEnumeratorTests
    {
        private readonly CallbackStub _callbackStub;

        public PropertyEnumeratorTests()
        {
            _callbackStub = new CallbackStub();
        }

        #region Simple Objects

        [Fact]
        public void Enumerate_NullObject_NullIsReturned()
        {
            var target = new HelperClasses.PropertyEnumerator(DefaultPredicate, _callbackStub.Callback);

            target.Enumerate(null);

            Assert.Equal(0, _callbackStub.Count);
        }

        [Fact]
        public void Enumerate_ClassWithString_SingleCallback()
        {
            var target = new HelperClasses.PropertyEnumerator(DefaultPredicate, _callbackStub.Callback);
            var principle = new ClassWithString
            {
                Badger = "Test Badger",
                Sausage = "Test Sausage"
            };

            target.Enumerate(principle);

            var callback = _callbackStub.CallbackObjects.Single();
            Assert.Equal("Test Badger", callback.Object.ToString());
            Assert.Equal("String", callback.PropertyInfo.PropertyType.Name);
        }

        [Fact]
        public void Enumerate_ClassWithPrimitives_NoCallbacks()
        {
            var target = new HelperClasses.PropertyEnumerator(DefaultPredicate, _callbackStub.Callback);
            var principle = new ClassWithPrimitives
            {
                IntProperty = 9,
                BoolProperty = true,
                CharProperty = 'J',
                DoubleProperty = 64.99
            };

            target.Enumerate(principle);

            Assert.Equal(0, _callbackStub.Count);
        }

        [Fact]
        public void Enumerate_ClassWithDate_SingleCallback()
        {
            var target = new HelperClasses.PropertyEnumerator(DefaultPredicate, _callbackStub.Callback);
            var date = DateTime.Now;
            var principle = new ClassWithDate
            {
                Badger = date,
                Sausage = DateTime.Now.AddSeconds(-64)
            };

            target.Enumerate(principle);

            var callback = _callbackStub.CallbackObjects.Single();
            Assert.Equal(date, callback.Object);
            Assert.Equal("DateTime", callback.PropertyInfo.PropertyType.Name);
        }

        #endregion

        #region Lists/Arrays

        [Fact]
        public void Enumerate_ClassWithObjectList_MultipleCallbacks()
        {
            var target = new HelperClasses.PropertyEnumerator(DefaultPredicate, _callbackStub.Callback);
            var principle = new ClassWithObjectList
            {
                ObjectList = new List<ClassWithString>
                {
                    new ClassWithString { Badger = "Badger 1", Sausage = "Sausage 1" },
                    new ClassWithString { Badger = "Badger 2", Sausage = null }
                }
            };

            target.Enumerate(principle);

            Assert.Equal(2, _callbackStub.Count);
        }

        [Fact]
        public void Enumerate_ClassWithLargeArray_MultipleCallbacks()
        {
            const int count = 100000;
            var target = new HelperClasses.PropertyEnumerator(DefaultPredicate, _callbackStub.Callback);
            var principle = new ClassWithObjectList
            {
                ObjectList = new()
            };

            for (var i = 0; i < count; i++)
            {
                principle.ObjectList.Add(new ClassWithString { Badger = $"Badge {i}", Sausage = $"Sausage {i}" });
            }

            target.Enumerate(principle);

            Assert.Equal(count, _callbackStub.Count);
        }

        [Fact]
        public void Enumerate_ClassWithStringList_NoCallbacks()
        {
            var target = new HelperClasses.PropertyEnumerator(DefaultPredicate, _callbackStub.Callback);
            var principle = new ClassWithStringList
            {
                StringList = new()
                {
                    "Badger 1",
                    "Badger 2"
                }
            };

            target.Enumerate(principle);

            Assert.Equal(0, _callbackStub.Count);
        }

        [Fact]
        public void Enumerate_ClassWithStringList_MultipleCallbacks()
        {
            var target = new HelperClasses.PropertyEnumerator((obj, prop) => prop.Name == "StringList" && obj.ToString() == "Badger 3", _callbackStub.Callback);
            var principle = new ClassWithStringList
            {
                StringList = new()
                {
                    "Badger 1",
                    "Badger 2",
                    "Badger 3",
                    "Badger 4",
                    "Badger 5"
                }
            };

            target.Enumerate(principle);

            Assert.Equal(1, _callbackStub.Count);
            Assert.All(_callbackStub.CallbackObjects, o => o?.ToString()?.StartsWith("Badger "));
        }

        #endregion

        #region Dictionary/Hash Tables

        [Fact]
        public void Enumerate_ClassWithSingleItemDictionary_SingleCallback()
        {
            var target = new HelperClasses.PropertyEnumerator(DefaultPredicate, _callbackStub.Callback);
            var principle = new ClassWithDictionary
            {
                DictionaryProperty = new()
                {
                    {"Key1", new ClassWithString {Badger = "Inside Badger 1"}},
                    {"Key2", new ClassWithString {Badger = "Badger Value 2"}}
                }
            };

            target.Enumerate(principle);

            Assert.Equal(2, _callbackStub.Count);
            Assert.Equal("Inside Badger 1", _callbackStub.CallbackObjects.First().Object.ToString());
        }

        [Fact]
        public void Enumerate_ClassWithKeyValuePairs_SingleCallback()
        {
            var target = new HelperClasses.PropertyEnumerator(DefaultPredicate, _callbackStub.Callback);
            var principle = new ClassWithKeyValuePair
            {
                KeyValuePair = new KeyValuePair<string, ClassWithString>("KeyValue_Key1", new ClassWithString { Badger = "Another Inner Badger" })
            };

            target.Enumerate(principle);

            var callback = _callbackStub.CallbackObjects.Single();
            Assert.Equal("Another Inner Badger", callback.Object.ToString());
            Assert.Equal("String", callback.PropertyInfo.PropertyType.Name);
        }

        #endregion

        #region Nested Objects

        [Fact]
        public void Enumerate_ClassWithNestedObjects_MultipleCallback()
        {
            var target = new HelperClasses.PropertyEnumerator(DefaultPredicate, _callbackStub.Callback);
            var principle = new ClassWithNestedObjects()
            {
                Badger = "Top Level Badger",
                ObjectList = new()
                {
                    CreateFoo("Inner Badger 1", null),
                    CreateFoo(null, "Inner Sausage 1"),
                    CreateFoo("Inner Badger 2", "Inner Sausage 2"),
                    CreateFoo(null, null)
                }
            };

            target.Enumerate(principle);

            Assert.Equal(3, _callbackStub.Count);
        }

        [Fact]
        public void Enumerate_ClassWithSingleNestedObject_SingleCallback()
        {
            static bool callbackFunction(object obj, PropertyInfo prop)
            {
                return prop.PropertyType.Name.Equals("Foo");
            }

            var target = new HelperClasses.PropertyEnumerator(callbackFunction, _callbackStub.Callback);
            var principle = new ClassWithNestedObjects()
            {
                Object = CreateFoo("Badger String", "Sausage String")
            };

            target.Enumerate(principle);

            var callback = _callbackStub.CallbackObjects.Single();
            Assert.Same(principle.Object, callback.Object);
            Assert.Equal(typeof(Foo), callback.PropertyInfo.PropertyType);
        }

        #endregion

        #region Attribute Tests

        [Fact]
        public void Enumerate_SearchForSingleAttribute_SingleCallback()
        {
            var target = new HelperClasses.PropertyEnumerator(AttributePredicate, _callbackStub.Callback);
            var principle = new ClassWithString
            {
                Badger = "Test Badger",
                Sausage = "Test Sausage"
            };

            target.Enumerate(principle);

            Assert.Equal("Test Sausage", _callbackStub.CallbackObjects.Single().Object.ToString());
        }

        [Fact]
        public void Enumerate_AttributesWithLargeArray_MultipleCallbacks()
        {
            const int count = 100000;
            var target = new HelperClasses.PropertyEnumerator(AttributePredicate, _callbackStub.Callback);
            var principle = new ClassWithObjectList
            {
                ObjectList = new()
            };

            for (var i = 0; i < count; i++)
            {
                principle.ObjectList.Add(new ClassWithString { Badger = $"Badge {i}", Sausage = $"Sausage {i}" });
            }

            target.Enumerate(principle);

            Assert.Equal(count, _callbackStub.Count);
        }

        #endregion

        #region Helper Methods

        private static bool DefaultPredicate(object obj, PropertyInfo prop) => prop.Name == "Badger";

        public static bool AttributePredicate(object obj, PropertyInfo prop) => prop.CustomAttributes.Any(ca => ca.AttributeType.Name == nameof(TestPropertyAttribute));

        private static Foo CreateFoo(string? badger, string? sausage)
        {
            return new Foo
            {
                BarProperty = new()
                {
                    ObjectList = new()
                    {
                        ObjectList = new()
                        {
                            new ClassWithString {Badger = badger, Sausage = sausage}
                        }
                    }
                },
                ActionProperty = TestAction,
                FuncProperty = TestFunc
            };
        }

        private static void TestAction(object obj) { }

        private static bool TestFunc() => true;

        #endregion
    }
}
