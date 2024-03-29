﻿namespace HelperClasses
{
    public class ObjectMapper
    {
        private readonly Dictionary<Type, object> _maps = new();

        public ObjectMapper AddMap<TSource, TDestination>(Func<TSource, TDestination> map)
        {
            if (Exists<TDestination>(typeof(TSource)))
            {
                throw new ArgumentException($"Source Type '{typeof(TSource).Name}' already exists for Destination Type '{typeof(TDestination).Name}'.");
            }

            var mapper = new ObjectMapper<TDestination>().AddMap(map);
            _maps.Add(typeof(TDestination), mapper);

            return this;
        }

        public bool Exists<TDestinationType>(Type? sourceType)
        {
            _ = sourceType ?? throw new ArgumentNullException(nameof(sourceType));

            var mapper = GetMapper<TDestinationType>();
            return mapper != null && mapper.Exists(sourceType);
        }

        public TDestination? Map<TDestination>(object? source)
        {
            if (source == null)
            {
                return default;
            }

            var mapper = GetMapper<TDestination>() ?? throw new InvalidMapRequest<TDestination>(source);
            return mapper.Map(source);
        }

        public IEnumerable<TDestination> Map<TDestination>(IEnumerable<object>? sources)
        {
            if (sources == null)
            {
                return Array.Empty<TDestination>();
            }

            var mapper = GetMapper<TDestination>() ?? throw new InvalidMapRequest<TDestination>(sources);
            return mapper.Map(sources);
        }

        public ObjectMapper<TDestination>? GetMapper<TDestination>() => _maps.ContainsKey(typeof(TDestination))
            ? (ObjectMapper<TDestination>)_maps[typeof(TDestination)]
            : default;
    }

    public class ObjectMapper<TDestination>
    {
        private readonly Dictionary<Type, Delegate> _maps = new();

        public ObjectMapper<TDestination> AddMap<TSource>(Func<TSource, TDestination>? map)
        {
            _ = map ?? throw new ArgumentNullException(nameof(map));

            if (Exists(typeof(TSource)))
            {
                throw new ArgumentException($"Source Type '{typeof(TSource).Name}' already exists.");
            }

            _maps.Add(typeof(TSource), map);
            return this;
        }

        public bool Exists(Type? sourceType)
        {
            _ = sourceType ?? throw new ArgumentNullException(nameof(sourceType));
            return _maps.ContainsKey(sourceType);
        }

        public TDestination Map(object source)
        {
            var sourceType = source.GetType();
            if (_maps.ContainsKey(sourceType))
            {
                Delegate sourceDelegate = _maps[sourceType] ?? throw new NullReferenceException(nameof(sourceType));
                return (TDestination)sourceDelegate.DynamicInvoke(source);
            }

            throw new InvalidMapRequest<TDestination>(source);
        }

        public IEnumerable<TDestination> Map(IEnumerable<object>? sources) => sources == null
            ? Array.Empty<TDestination>()
            : sources.Select(Map);
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