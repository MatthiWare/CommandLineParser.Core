namespace MatthiWare.CommandLine.Abstractions.Command
{
    /// <summary>
    /// Command configuration options
    /// </summary>
    public interface ICommandLineCommand : ICommandLineOption
    {
        bool AutoExecute { get; }
    }
}
