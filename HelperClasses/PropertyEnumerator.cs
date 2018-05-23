using System;
using System.Collections;
using System.Reflection;

namespace HelperClasses
{
    public class PropertyEnumerator
    {
        private readonly Func<object, PropertyInfo, bool> _predicate;
        private readonly Action<object> _callback;

        public PropertyEnumerator(Func<object, PropertyInfo, bool> predicate, Action<object> callback)
        {
            _predicate = predicate;
            _callback = callback;
        }

        public void Transverse(object obj)
        {
            foreach(var property in obj.GetType().GetProperties())
            {
                Transverse(property.GetValue(obj), property);
            }
        }

        public void Transverse(object obj, PropertyInfo prop)
        {
            if (obj == null)
            {
                return;
            }

            if (_predicate.Invoke(obj, prop))
            {
                _callback.Invoke(obj);
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
                    Transverse(child, prop);
                }
                return;
            }

            if (prop.PropertyType.IsClass || prop.PropertyType.IsValueType)
            {
                foreach (var childProperties in obj.GetType().GetProperties())
                {
                    Transverse(childProperties.GetValue(obj), childProperties);
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
