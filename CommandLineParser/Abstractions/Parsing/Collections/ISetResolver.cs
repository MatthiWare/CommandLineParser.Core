using MatthiWare.CommandLine.Abstractions.Models;
using System.Collections.Generic;

namespace MatthiWare.CommandLine.Abstractions.Parsing.Collections
{
    /// <inheritdoc/>
    public interface ISetResolver<TModel> : ICommandLineArgumentResolver
    {
        /// <summary>
        /// Resolves the argument from the model
        /// </summary>
        /// <param name="model">Argument model</param>
        /// <returns>The resolved type</returns>
        new HashSet<TModel> Resolve(ArgumentModel model);
    }
}
