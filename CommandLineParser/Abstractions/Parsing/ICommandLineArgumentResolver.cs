using MatthiWare.CommandLine.Abstractions.Models;

namespace MatthiWare.CommandLine.Abstractions.Parsing
{
    /// <summary>
    /// Argument resolver
    /// </summary>
    public interface ICommandLineArgumentResolver
    {
        /// <summary>
        /// Checks if the resolver can resolve the argument
        /// </summary>
        /// <param name="model">argument</param>
        /// <returns>True if it can resolve it correctly</returns>
        bool CanResolve(ArgumentModel model);

        /// <summary>
        /// Resolves the argument from the model
        /// </summary>
        /// <param name="model">Argument model</param>
        /// <returns>The resolved type</returns>
        object Resolve(ArgumentModel model);
    }

    /// <summary>
    /// Generic argument resolver
    /// </summary>
    /// <typeparam name="T">Argument type</typeparam>
    public interface ICommandLineArgumentResolver<T> : ICommandLineArgumentResolver
    {
        /// <summary>
        /// Resolves the argument from the model
        /// </summary>
        /// <param name="model">Argument model</param>
        /// <returns>The resolved type</returns>
        new T Resolve(ArgumentModel model);
    }
}
