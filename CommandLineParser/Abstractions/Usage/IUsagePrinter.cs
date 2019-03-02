using MatthiWare.CommandLine.Abstractions.Command;

namespace MatthiWare.CommandLine.Abstractions.Usage
{
    /// <summary>
    /// CLI Usage Output Printer
    /// </summary>
    public interface IUsagePrinter
    {
        /// <summary>
        /// Print global usage
        /// </summary>
        void PrintUsage();
        /// <summary>
        /// Print an argument
        /// </summary>
        /// <param name="argument">The given argument</param>
        void PrintUsage(IArgument argument);

        /// <summary>
        /// Print command usage
        /// </summary>
        /// <param name="command">The given command</param>
        void PrintUsage(ICommandLineCommand command);

        /// <summary>
        /// Print option usage
        /// </summary>
        /// <param name="option">The given option</param>
        void PrintUsage(ICommandLineOption option);
    }
}
