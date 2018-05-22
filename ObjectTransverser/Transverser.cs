using System;
using System.Collections;
using System.Reflection;

namespace ObjectTransverser
{
    public class Transverser
    {
        private readonly Func<object, PropertyInfo, bool> _predicate;
        private readonly Action<object> _callback;

        public Transverser(Func<object, PropertyInfo, bool> predicate, Action<object> callback)
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

            if (obj is IEnumerable)
            {
                foreach(var child in (IEnumerable)obj)
                {
                    Transverse(child, prop);
                }
                return;
            }

            if (obj.GetType().IsClass)
            {
                foreach (var childProperties in obj.GetType().GetProperties())
                {
                    Transverse(childProperties.GetValue(obj), childProperties);
                }
            }
        }

        private bool IsNotValid(object obj)
        {
            return obj.GetType().IsPrimitive 
                || obj is string;
        }
    }
}
