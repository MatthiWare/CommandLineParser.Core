using System;
using System.Collections.Generic;
using System.Text;
using MatthiWare.CommandLine.Abstractions.Parsing;
using MatthiWare.CommandLine.Core.Parsing.Resolvers;

namespace MatthiWare.CommandLine.Core.Parsing
{
    public class ResolverFactory : IResolverFactory
    {

        private IDictionary<Type, Type> m_types = new Dictionary<Type, Type>();
        private IDictionary<Type, object> m_cache = new Dictionary<Type, object>();

        public bool Contains<T>()
            => Contains(typeof(T));

        public bool Contains(Type argument)
            => m_types.ContainsKey(argument);

        public ICommandLineArgumentResolver<T> CreateResolver<T>()
        {
            Type argType = typeof(T);

            if (!m_cache.ContainsKey(argType))
            {
                var instance = (ICommandLineArgumentResolver<T>)Activator.CreateInstance(m_types[argType]);

                m_cache.Add(argType, instance);
            }

            return (ICommandLineArgumentResolver<T>)m_cache[argType];
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
