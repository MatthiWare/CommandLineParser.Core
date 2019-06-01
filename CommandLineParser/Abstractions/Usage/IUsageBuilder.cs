using System.Collections.Generic;
using MatthiWare.CommandLine.Abstractions.Command;

namespace MatthiWare.CommandLine.Abstractions.Usage
{
    public interface IUsageBuilder
    {
        void Print();
        void PrintUsage(string name, bool hasOptions, bool hasCommands);
        void PrintOptions(IEnumerable<ICommandLineOption> options);
        void PrintOption(ICommandLineOption option);
        void PrintCommandDescriptions(IEnumerable<ICommandLineCommand> commands);
        void PrintCommandDescription(ICommandLineCommand command);
        void PrintCommand(string name, ICommandLineCommandContainer container);
    }
}
