using System;
using System.Collections.Generic;

namespace HelperClasses
{
    public class ObjectMapper<TDestination>
    {
        private Dictionary<Type, Delegate> _maps = new Dictionary<Type, Delegate>();

        public void AddMap<TSource>(Func<TSource, TDestination> map)
        {
            _maps.Add(typeof(TSource), map);
        }

        public TDestination Map(object source)
        {
            var sourceType = source.GetType();
            if (_maps.ContainsKey(sourceType))
            {
                return (TDestination)_maps[sourceType].DynamicInvoke(source);
            }

            throw new InvalidMapRequest<TDestination>(source);
        }
    }

    public class InvalidMapRequest<TDesstination> : Exception
    {
        public Type SourceType { get; }
        public Type DestinationType { get; }
        public object SourceObject { get; }

        public InvalidMapRequest(object obj):
            base("A valid map does not exist for the TSource spcified")
        {
            SourceType = obj.GetType();
            DestinationType = typeof(TDesstination);
            SourceObject = obj;
        }
    }
}
