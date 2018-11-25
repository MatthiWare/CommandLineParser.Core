using MatthiWare.CommandLine.Abstractions.Models;

namespace MatthiWare.CommandLine.Abstractions.Parsing
{
    public interface ICommandLineArgumentResolver
    {
        bool CanResolve(ArgumentModel model);

        object Resolve(ArgumentModel model);
    }

    public interface ICommandLineArgumentResolver<T> : ICommandLineArgumentResolver
    {
        new T Resolve(ArgumentModel model);
    }
}
