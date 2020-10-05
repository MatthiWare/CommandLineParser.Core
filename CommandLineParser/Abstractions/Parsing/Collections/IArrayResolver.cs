using MatthiWare.CommandLine.Abstractions.Models;

namespace MatthiWare.CommandLine.Abstractions.Parsing.Collections
{
    /// <summary>
    /// Resolve array types
    /// </summary>
    public interface IArrayResolver<TModel> : ICommandLineArgumentResolver
    {
        /// <summary>
        /// Resolves the argument from the model
        /// </summary>
        /// <param name="model">Argument model</param>
        /// <returns>The resolved type</returns>
        new TModel[] Resolve(ArgumentModel model);
    }
}
