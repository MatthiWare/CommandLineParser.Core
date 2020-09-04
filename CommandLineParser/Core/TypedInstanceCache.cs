using MatthiWare.CommandLine.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MatthiWare.CommandLine.Core
{
    /// <summary>
    /// A strongly-typed instance cache
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public class TypedInstanceCache<TValue>
    {
        private readonly IServiceProvider serviceProvider;
        private readonly Dictionary<Type, InstanceMetadata<TValue>> instances = new Dictionary<Type, InstanceMetadata<TValue>>();

        public TypedInstanceCache(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Adds a new instance to the cache
        /// </summary>
        /// <param name="value"></param>
        public void Add(TValue value)
        {
            var key = typeof(TValue);

            if (!instances.ContainsKey(key))
                instances.Add(typeof(TValue), new InstanceMetadata<TValue>(key, value));
            else
                instances[key].SetInstance(value);
        }

        /// <summary>
        /// Adds a type to the cache that will be resolved later
        /// </summary>
        /// <param name="type"></param>
        public void Add(Type type)
        {
            if (!instances.ContainsKey(type))
                instances.Add(typeof(TValue), new InstanceMetadata<TValue>(type));
            else
                instances[type].Clear();
        }

        /// <summary>
        /// Gets the values from the cache. This will instantiate unresolved items. 
        /// </summary>
        /// <returns></returns>
        public IReadOnlyList<TValue> Get()
        {
            var toResolve = instances.Values.Where(meta => !meta.Created).ToArray();

            foreach (var meta in toResolve)
            {
                var instance = ActivatorUtilities.GetServiceOrCreateInstance(this.serviceProvider, meta.Type);

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
