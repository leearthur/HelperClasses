using System;
using System.Collections;
using System.Reflection;

namespace HelperClasses
{
    public class PropertyEnumerator
    {
        private readonly Func<object, PropertyInfo, bool> _predicate;
        private readonly Action<object, PropertyInfo> _callback;

        public PropertyEnumerator(Func<object, PropertyInfo, bool> predicate, Action<object, PropertyInfo> callback)
        {
            _predicate = predicate;
            _callback = callback;
        }

        public void Enumerate(object obj)
        {
            if (obj == null)
            {
                return;
            }

            foreach (var property in obj.GetType().GetProperties())
            {
                Enumerate(property.GetValue(obj), property);
            }
        }

        public void Enumerate(object obj, PropertyInfo prop)
        {
            if (obj == null)
            {
                return;
            }

            if (_predicate.Invoke(obj, prop))
            {
                _callback.Invoke(obj, prop);
                return;
            }

            if (IsNotValid(obj))
            {
                return;
            }

            if (obj is IEnumerable enumerable)
            {
                foreach (var child in enumerable)
                {
                    Enumerate(child, prop);
                }
                return;
            }

            var objType = obj.GetType();
            if (objType.IsClass || prop.PropertyType.IsClass || prop.PropertyType.IsValueType)
            {
                foreach (var childProperties in objType.GetProperties())
                {
                    Enumerate(childProperties.GetValue(obj), childProperties);
                }
            }
        }

        private static bool IsNotValid(object obj)
        {
            return obj.GetType().IsPrimitive
                || obj is string
                || obj is DateTime
                || obj is Delegate;
        }
    }
}