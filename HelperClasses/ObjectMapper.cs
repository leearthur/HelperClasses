using System;
using System.Collections.Generic;
using System.Linq;

namespace HelperClasses
{
    public class ObjectMapper<TDestination>
    {
        private readonly Dictionary<Type, Delegate> _maps = new Dictionary<Type, Delegate>();

        public void AddMap<TSource>(Func<TSource, TDestination> map)
        {
            if (map == null)
            {
                throw new ArgumentNullException(nameof(map));
            }

            if (Exists(typeof(TSource)))
            {
                throw new ArgumentException("Specified Source Type already exists");
            }

            _maps.Add(typeof(TSource), map);
        }

        public bool Exists(Type sourceType)
        {
            if (sourceType == null)
            {
                throw new ArgumentNullException(nameof(sourceType));
            }

            return _maps.ContainsKey(sourceType);
        }

        public TDestination Map(object source)
        {
            if (source == null)
            {
                return default(TDestination);
            }

            var sourceType = source.GetType();
            if (_maps.ContainsKey(sourceType))
            {
                return (TDestination)_maps[sourceType].DynamicInvoke(source);
            }

            throw new InvalidMapRequest<TDestination>(source);
        }

        public IEnumerable<TDestination> Map(IEnumerable<object> sources)
        {
            if (sources == null)
            {
                return new TDestination[0];
            }

            return sources.Select(Map);
        }
    }

    [Serializable]
    public class InvalidMapRequest<TDesstination> : Exception
    {
        public Type SourceType { get; }
        public Type DestinationType { get; }
        public object SourceObject { get; }

        public InvalidMapRequest(object obj) :
            base("A valid map does not exist for the TSource spcified")
        {
            SourceType = obj.GetType();
            DestinationType = typeof(TDesstination);
            SourceObject = obj;
        }
    }
}