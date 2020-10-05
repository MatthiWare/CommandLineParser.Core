using MatthiWare.CommandLine.Abstractions.Models;
using MatthiWare.CommandLine.Abstractions.Parsing;
using MatthiWare.CommandLine.Abstractions.Parsing.Collections;
using Microsoft.Extensions.Logging;
using System;

namespace MatthiWare.CommandLine.Core.Parsing.Resolvers.Collections
{
    /// <inheritdoc />
    internal class ArrayResolver<TModel> : IArrayResolver<TModel>
    {
        private readonly ILogger<CommandLineParser> logger;
        private readonly IArgumentResolver<TModel> resolver;

        /// <inheritdoc />
        public ArrayResolver(ILogger<CommandLineParser> logger, IArgumentResolver<TModel> resolver)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
        }

        /// <inheritdoc />
        public bool CanResolve(ArgumentModel model)
        {
            if (model is null)
            {
                return false;
            }

            foreach (var input in model.Values)
            {
                if (!resolver.CanResolve(input))
                {
                    return false;
                }
            }

            return true;
        }

        /// <inheritdoc />
        public bool CanResolve(string value)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public TModel[] Resolve(ArgumentModel model)
        {
            var array = Array.CreateInstance(typeof(TModel), model.Values.Count);

            for (int i = 0; i < model.Values.Count; i++)
            {
                var value = resolver.Resolve(model.Values[i]);
                array.SetValue(value, i);
            }

            return (TModel[])array;
        }

        /// <inheritdoc />
        public object Resolve(string value)
        {
            throw new NotImplementedException();
        }

        object ICommandLineArgumentResolver.Resolve(ArgumentModel model)
            => Resolve(model);
    }
}
