using MatthiWare.CommandLine.Abstractions.Models;

namespace MatthiWare.CommandLine.Abstractions.Parsing.Collections
{
    /// <summary>
    /// Resolve array types
    /// </summary>
    public interface IArrayResolver<TModel> : ICommandLineArgumentResolver
    {
        new TModel[] Resolve(ArgumentModel model);
    }
}
