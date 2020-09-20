namespace MatthiWare.CommandLine.Abstractions.Parsing
{
    /// <summary>
    /// Argument resolver
    /// </summary>
    /// <typeparam name="TArgument">Argument type</typeparam>
    public interface IArgumentResolver<TArgument>
        : ICommandLineArgumentResolver<TArgument>, ICommandLineArgumentResolver
    {
    }
}
