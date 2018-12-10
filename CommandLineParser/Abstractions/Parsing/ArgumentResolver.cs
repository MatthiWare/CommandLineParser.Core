using MatthiWare.CommandLine.Abstractions.Models;

namespace MatthiWare.CommandLine.Abstractions.Parsing
{
    /// <summary>
    /// Class to resolve arguments
    /// </summary>
    /// <typeparam name="TArgument">Argument type</typeparam>
    public abstract class ArgumentResolver<TArgument>
        : ICommandLineArgumentResolver<TArgument>, ICommandLineArgumentResolver
    {
        /// <summary>
        /// Checks if the resolver can resolve the argument
        /// </summary>
        /// <param name="model">argument</param>
        /// <returns>True if it can resolve it correctly</returns>
        public abstract bool CanResolve(ArgumentModel model);

        /// <summary>
        /// Resolves the argument from the model
        /// </summary>
        /// <param name="model">Argument model</param>
        /// <returns>The resolved type</returns>
        public abstract TArgument Resolve(ArgumentModel model);

        /// <summary>
        /// Resolves the argument from the model
        /// </summary>
        /// <param name="model">Argument model</param>
        /// <returns>The resolved type</returns>
        object ICommandLineArgumentResolver.Resolve(ArgumentModel model) => Resolve(model);
    }
}
