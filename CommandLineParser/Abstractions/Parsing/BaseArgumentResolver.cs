using MatthiWare.CommandLine.Abstractions.Models;
using System.Linq;

namespace MatthiWare.CommandLine.Abstractions.Parsing
{
    /// <summary>
    /// Class to resolve arguments
    /// </summary>
    /// <typeparam name="TArgument">Argument type</typeparam>
    public abstract class BaseArgumentResolver<TArgument> 
        : IArgumentResolver<TArgument>
    {
        /// <inheritdoc/>
        public virtual bool CanResolve(ArgumentModel model) => CanResolve(model.Values.FirstOrDefault());

        /// <inheritdoc/>
        public abstract bool CanResolve(string value);

        /// <inheritdoc/>
        public virtual TArgument Resolve(ArgumentModel model) => Resolve(model.Values.FirstOrDefault());

        /// <inheritdoc/>
        public abstract TArgument Resolve(string value);

        object ICommandLineArgumentResolver.Resolve(ArgumentModel model) => Resolve(model);

        object ICommandLineArgumentResolver.Resolve(string value) => Resolve(value);
    }
}
