using MatthiWare.CommandLine.Abstractions.Models;
using System.Collections.Generic;

namespace MatthiWare.CommandLine.Abstractions.Parsing.Collections
{
    /// <inheritdoc/>
    public interface IListResolver<TModel> : ICommandLineArgumentResolver
    {
        /// <summary>
        /// Resolves the argument from the model
        /// </summary>
        /// <param name="model">Argument model</param>
        /// <returns>The resolved type</returns>
        new List<TModel> Resolve(ArgumentModel model);
    }
}
