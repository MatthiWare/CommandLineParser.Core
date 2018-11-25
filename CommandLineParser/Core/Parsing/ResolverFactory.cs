using System;
using System.Collections.Generic;
using MatthiWare.CommandLine.Abstractions.Parsing;
using MatthiWare.CommandLine.Core.Parsing.Resolvers;

namespace MatthiWare.CommandLine.Core.Parsing
{
    internal class ResolverFactory : IResolverFactory
    {
        private IDictionary<Type, Type> m_types = new Dictionary<Type, Type>();
        private IDictionary<Type, object> m_cache = new Dictionary<Type, object>();

        public ResolverFactory()
        {
            Register<string, StringResolver>();
            Register<bool, BoolResolver>();
            Register<int, IntResolver>();
            Register<double, DoubleResolver>();
        }

        public bool Contains<T>()
            => Contains(typeof(T));

        public bool Contains(Type argument)
            => m_types.ContainsKey(argument);

        public ICommandLineArgumentResolver<T> CreateResolver<T>()
        {
            return (ICommandLineArgumentResolver<T>)CreateResolver(typeof(T));
        }

        public ICommandLineArgumentResolver CreateResolver(Type type)
        {
            if (!m_cache.ContainsKey(type))
            {
                var instance = (ICommandLineArgumentResolver)Activator.CreateInstance(m_types[type]);

                m_cache.Add(type, instance);
            }

            return (ICommandLineArgumentResolver)m_cache[type];
        }

        public void Register<TArgument>(ICommandLineArgumentResolver<TArgument> resolverInstance, bool overwrite = false)
        {
            Register<TArgument, ICommandLineArgumentResolver<TArgument>>(overwrite);

            var typeKey = typeof(TArgument);

            if (overwrite && m_cache.ContainsKey(typeKey))
                m_cache.Remove(typeKey);

            m_cache.Add(typeKey, resolverInstance);
        }

        public void Register<TArgument, TResolver>(bool overwrite = false) where TResolver : ICommandLineArgumentResolver<TArgument>
            => Register(typeof(TArgument), typeof(TResolver), overwrite);

        public void Register(Type argument, Type resolver, bool overwrite = false)
        {
            if (overwrite && Contains(argument))
                m_types.Remove(argument);

            m_types.Add(argument, resolver);
        }
    }
}
