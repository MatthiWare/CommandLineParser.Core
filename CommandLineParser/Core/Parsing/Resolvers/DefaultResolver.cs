using MatthiWare.CommandLine.Abstractions.Models;
using MatthiWare.CommandLine.Abstractions.Parsing;
using System;
using System.Linq;
using System.Reflection;

namespace MatthiWare.CommandLine.Core.Parsing.Resolvers
{
    internal class DefaultResolver<T> : ArgumentResolver<T>
    {
        private readonly Type m_type;

        public override bool CanResolve(ArgumentModel model)
        {
            return TryResolve(model, out _);
        }

        public override T Resolve(ArgumentModel model)
        {
            TryResolve(model, out T instance);

            return instance;
        }

        private bool TryResolve(ArgumentModel model, out T result)
        {
            if (!model.HasValue)
            {
                result = default;
                return false;
            }

            var ctors = typeof(T).GetConstructors();
            var ctor = ctors
                .Where(ct => ct.GetParameters().Count() == 1 && ct.GetParameters().First().ParameterType == typeof(string))
                .FirstOrDefault();

            if (ctor == null)
            {
                result = default;
                return false;
            }

            result = (T)Activator.CreateInstance(typeof(T), model.Value);
            return true;
        }
    }
}
