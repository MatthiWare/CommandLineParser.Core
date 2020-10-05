using MatthiWare.CommandLine.Abstractions.Models;
using MatthiWare.CommandLine.Abstractions.Parsing;
using MatthiWare.CommandLine.Abstractions.Parsing.Collections;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace MatthiWare.CommandLine.Core.Parsing.Resolvers.Collections
{
    /// <inheritdoc/>
    public sealed class SetResolver<TModel> : ISetResolver<TModel>
    {
        private readonly ILogger<CommandLineParser> logger;
        private readonly IArgumentResolver<TModel> resolver;

        /// <inheritdoc/>
        public SetResolver(ILogger<CommandLineParser> logger, IArgumentResolver<TModel> resolver)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
        }

        /// <inheritdoc/>
        public bool CanResolve(ArgumentModel model)
        {
            if (model is null)
            {
                return false;
            }

            foreach (var value in model.Values)
            {
                if (!resolver.CanResolve(value))
                {
                    return false;
                }
            }

            return true;
        }

        /// <inheritdoc/>
        public bool CanResolve(string value)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public HashSet<TModel> Resolve(ArgumentModel model)
        {
            var list = new HashSet<TModel>();

            foreach (var value in model.Values)
            {
                list.Add(resolver.Resolve(value));
            }

            return list;
        }

        /// <inheritdoc/>
        public object Resolve(string value)
        {
            throw new NotImplementedException();
        }

        object ICommandLineArgumentResolver.Resolve(ArgumentModel model) => Resolve(model);

    }
}
