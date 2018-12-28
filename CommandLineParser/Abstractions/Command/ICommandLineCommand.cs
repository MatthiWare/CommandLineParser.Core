namespace MatthiWare.CommandLine.Abstractions.Command
{
    /// <summary>
    /// Command configuration options
    /// </summary>
    public interface ICommandLineCommand : IArgument
    {
        string Name { get; }
        bool IsRequired { get; }
        string Description { get; }
        bool AutoExecute { get; }
    }
}
