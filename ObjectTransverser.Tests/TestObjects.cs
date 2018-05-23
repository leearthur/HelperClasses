using System;
using System.Collections.Generic;

namespace HelperClasses.Tests
{
    public class ClassWithString
    {
        public string Badger { get; set; }

        [TestProperty]
        public string Sausage { get; set; }
    }

    public class ClassWithDate
    {
        public DateTime Badger { get; set; }
        public DateTime Sausage { get; set; }
    }

    public class ClassWithObjectList
    {
        public List<ClassWithString> ObjectList { get; set; }
    }

    public class ClassWithStringList
    {
        public List<string> StringList { get; set; }
    }

    public class ClassWithPrimitives
    {
        public int IntProperty { get; set; }

        public double DoubleProperty { get; set; }

        public bool BoolProperty { get; set; }

        public char CharProperty { get; set; }
    }

    public class ClassWithDictionary
    {
        public Dictionary<string, ClassWithString> DictionaryProperty { get; set; }
    }

    public class ClassWithKeyValuePair
    {
        public KeyValuePair<string, ClassWithString> KeyValuePair { get; set; }
    }

    public class ClassWithNestedObjects
    {
        public string Badger { get; set; }

        public List<Foo> ObjectList { get; set; }
    }

    public class Foo
    {
        public Bar BarProperty { get; set; }

        public Func<bool> FuncProperty { get; set; }

        public Action<object> ActionProperty { get; set; }
    }

    public class Bar
    {
        public ClassWithObjectList ObjectList { get; set; }
    }
}
