namespace MatthiWare.CommandLine.Abstractions.Command
{
    /// <summary>
    /// Command configuration options
    /// </summary>
    public interface ICommandLineCommand : IArgument
    {
        /// <summary>
        /// Name of the command
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Indicates if the command is required or not
        /// </summary>
        bool IsRequired { get; }

        /// <summary>
        /// Description of the command
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Auto executes the command if set to true
        /// </summary>
        bool AutoExecute { get; }
    }
}
