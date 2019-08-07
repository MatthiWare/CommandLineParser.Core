using MatthiWare.CommandLine.Abstractions;
using MatthiWare.CommandLine.Abstractions.Validations;
using System;
using System.Collections.Generic;
using System.Text;

namespace MatthiWare.CommandLine.Core.Validations
{
    internal class ValidatorsContainer : IValidatorsContainer
    {
        private IDictionary<Type, Type> m_types = new Dictionary<Type, Type>();
        private IDictionary<Type, object> m_cache = new Dictionary<Type, object>();

        private readonly IContainerResolver containerResolver;

        public ValidatorsContainer(IContainerResolver containerResolver)
        {
            this.containerResolver = containerResolver;
        }

        public void AddValidator<TKey>(IValidator<TKey> validator)
        {
            m_cache.Add(typeof(TKey), validator);
        }

        public void AddValidator<TKey, V>() where V : IValidator<TKey>
        {
            m_types.Add(typeof(TKey), typeof(V));
        }

        public IValidator<T> GetValidatorFor<T>()
        {
            var key = typeof(T);

            if (!m_cache.ContainsKey(key))
            {
                var instance = containerResolver.Resolve(key);

                m_cache.Add(key, instance);
            }

            return (IValidator<T>)m_cache[key];
        }

        public bool HasValidatorFor<T>() => m_types.ContainsKey(typeof(T));
    }
}
