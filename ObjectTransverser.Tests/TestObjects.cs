using System.Collections.Generic;

namespace ObjectTransverser.Tests
{
    public class ClassWithString
    {
        public string Badger { get; set; }

        public string Sausage { get; set; }
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
}
