using MatthiWare.CommandLine.Abstractions;
using MatthiWare.CommandLine.Abstractions.Validations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MatthiWare.CommandLine.Core.Validations
{
    internal class ValidatorsContainer : IValidatorsContainer
    {
        private Dictionary<Type, List<Type>> m_types = new Dictionary<Type, List<Type>>();
        private Dictionary<Type, List<(Type key, IValidator validator)>> m_cache = new Dictionary<Type, List<(Type, IValidator)>>();

        private readonly IContainerResolver containerResolver;

        public ValidatorsContainer(IContainerResolver containerResolver)
        {
            this.containerResolver = containerResolver;
        }

        public void AddValidator<TKey>(IValidator<TKey> validator)
        {
            var key = typeof(TKey);

            GetOrCreateTypeListFor(key).Add(validator.GetType());
            GetOrCreateCacheListFor(key).Add((key, validator));
        }

        public void AddValidator<TKey, V>() where V : IValidator<TKey> => GetOrCreateTypeListFor(typeof(TKey)).Add(typeof(V));

        public IReadOnlyCollection<IValidator> GetValidators<T>()
        {
            var key = typeof(T);

            return GetValidators(key);
        }

        public IReadOnlyCollection<IValidator> GetValidators(Type key)
        {
            var types = GetOrCreateTypeListFor(key);
            var instances = GetOrCreateCacheListFor(key);

            var typesNotInList = types.Except(instances.Select(kvp => kvp.key)).ToArray();

            foreach (var type in typesNotInList)
            {
                var instance = (IValidator)containerResolver.Resolve(type);

                instances.Add((type, instance));
            }

            return instances.Select(kvp => kvp.validator).ToArray();
        }

        public bool HasValidatorFor<T>() => HasValidatorFor(typeof(T));

        public bool HasValidatorFor(Type type) => m_types.ContainsKey(type);

        private List<(Type key, IValidator validator)> GetOrCreateCacheListFor(Type key)
        {
            if (!m_cache.TryGetValue(key, out List<(Type, IValidator)> instances))
            {
                instances = new List<(Type, IValidator)>();
                m_cache.Add(key, instances);
            }

            return instances;
        }

        private List<Type> GetOrCreateTypeListFor(Type key)
        {
            if (!m_types.TryGetValue(key, out List<Type> types))
            {
                types = new List<Type>();
                m_types.Add(key, types);
            }

            return types;
        }
    }
}
