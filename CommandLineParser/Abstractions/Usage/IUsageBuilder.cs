using System.Collections.Generic;
using MatthiWare.CommandLine.Abstractions.Command;

namespace MatthiWare.CommandLine.Abstractions.Usage
{
    public interface IUsageBuilder
    {
        void Print();
        void PrintUsage(string name, bool hasOptions, bool hasCommands);
        void PrintOptions(IEnumerable<ICommandLineOption> options, int descriptionShift = 4);
        void PrintOption(ICommandLineOption option, int descriptionShift = 4);
        void PrintCommandDescriptions(IEnumerable<ICommandLineCommand> commands, int descriptionShift = 4);
        void PrintCommandDescription(ICommandLineCommand command, int descriptionShift = 4);
        void PrintCommand(string name, ICommandLineCommandContainer container);
    }
}
