using MatthiWare.CommandLine.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MatthiWare.CommandLine.Core
{
    public class TypedInstanceCache<TValue>
    {
        private readonly IContainerResolver resolver;
        private readonly Dictionary<Type, InstanceMetadata<TValue>> instances = new Dictionary<Type, InstanceMetadata<TValue>>();

        public TypedInstanceCache(IContainerResolver resolver)
        {
            this.resolver = resolver;
        }

        public void Add(TValue value)
        {
            var key = typeof(TValue);

            if (!instances.ContainsKey(key))
                instances.Add(typeof(TValue), new InstanceMetadata<TValue>(key, value));
            else
                instances[key].SetInstance(value);
        }

        public void Add(Type type)
        {
            if (!instances.ContainsKey(type))
                instances.Add(typeof(TValue), new InstanceMetadata<TValue>(type));
            else
                instances[type].Clear();
        }

        public void Add<T>() 
            where T : TValue
        {
            Add(typeof(T));
        }

        public IReadOnlyList<TValue> Get()
        {
            var toResolve = instances.Values.Where(meta => !meta.Created).ToArray();

            foreach (var meta in toResolve)
            {
                var instance = resolver.Resolve(meta.Type);

                meta.SetInstance(instance);
            }

            return instances.Values.Select(meta => meta.Instance).ToList();
        }

        private class InstanceMetadata<T>
        {
            public bool Created { get; private set; }
            public T Instance { get; private set; }

            public readonly Type Type;

            public InstanceMetadata(Type type, T instance)
            {
                Type = type;

                SetInstance(instance);
            }

            public InstanceMetadata(Type type)
            {
                Type = type;

                Clear();
            }

            public void SetInstance(object instance)
            {
                Instance = (T)instance;
                Created = true;
            }

            public void Clear()
            {
                Instance = default;
                Created = false;
            }
        }
    }
}
