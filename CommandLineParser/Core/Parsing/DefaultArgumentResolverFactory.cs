using System;
using System.Collections.Generic;
using MatthiWare.CommandLine.Abstractions;
using MatthiWare.CommandLine.Abstractions.Parsing;
using MatthiWare.CommandLine.Core.Parsing.Resolvers;
using MatthiWare.CommandLine.Core.Utils;

namespace MatthiWare.CommandLine.Core.Parsing
{
    internal class DefaultArgumentResolverFactory : IArgumentResolverFactory
    {
        private IDictionary<Type, Type> m_types = new Dictionary<Type, Type>();
        private IDictionary<Type, object> m_cache = new Dictionary<Type, object>();

        private readonly IContainerResolver containerResolver;

        public DefaultArgumentResolverFactory(IContainerResolver containerResolver)
        {
            this.containerResolver = containerResolver;

            Register<string, StringResolver>();
            Register<bool, BoolResolver>();
            Register<int, IntResolver>();
            Register<double, DoubleResolver>();
            Register(typeof(Enum), typeof(EnumResolver<>));
        }

        public bool Contains<T>()
            => Contains(typeof(T));

        public bool Contains(Type argument)
            => m_types.ContainsKey(argument.IsEnum ? typeof(Enum) : argument);

        public ICommandLineArgumentResolver<T> CreateResolver<T>()
        {
            return (ICommandLineArgumentResolver<T>)CreateResolver(typeof(T));
        }

        public ICommandLineArgumentResolver CreateResolver(Type type)
        {
            if (!m_cache.ContainsKey(type))
            {
                bool isEnum = type.IsEnum;

                var instance = isEnum ?
                    (ICommandLineArgumentResolver)containerResolver.Resolve(m_types[typeof(Enum)].MakeGenericType(type)) :
                    (ICommandLineArgumentResolver)containerResolver.Resolve(m_types[type]);

                m_cache.Add(type, instance);
            }

            return (ICommandLineArgumentResolver)m_cache[type];
        }

        public void Register<TArgument>(ArgumentResolver<TArgument> resolverInstance, bool overwrite = false)
        {
            Register<TArgument, ArgumentResolver<TArgument>>(overwrite);

            var typeKey = typeof(TArgument);

            if (overwrite && m_cache.ContainsKey(typeKey))
                m_cache.Remove(typeKey);

            m_cache.Add(typeKey, resolverInstance);
        }

        public void Register<TArgument, TResolver>(bool overwrite = false) where TResolver : ArgumentResolver<TArgument>
            => Register(typeof(TArgument), typeof(TResolver), overwrite);

        public void Register(Type argument, Type resolver, bool overwrite = false)
        {
            if (!resolver.IsAssignableToGenericType(typeof(ArgumentResolver<>)))
                throw new InvalidCastException($"The given resolver is not assignable from {typeof(ArgumentResolver<>)}");

            if (overwrite && Contains(argument))
                m_types.Remove(argument);

            m_types.Add(argument, resolver);
        }
    }
}
