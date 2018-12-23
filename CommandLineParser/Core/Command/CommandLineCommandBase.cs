using System.Collections.Generic;
using System.Text;
using MatthiWare.CommandLine.Abstractions;
using MatthiWare.CommandLine.Abstractions.Command;
using MatthiWare.CommandLine.Abstractions.Parsing;
using MatthiWare.CommandLine.Abstractions.Parsing.Command;
using MatthiWare.CommandLine.Abstractions.Usage;

namespace MatthiWare.CommandLine.Core.Command
{
    internal abstract class CommandLineCommandBase :
        ICommandLineCommandParser,
        ICommandLineCommandContainer,
        ICommandLineCommand
    {
        protected readonly List<CommandLineOptionBase> m_options = new List<CommandLineOptionBase>();
        protected readonly List<ICommandLineCommand> m_commands = new List<ICommandLineCommand>();

        public IReadOnlyList<ICommandLineOption> Options => m_options.AsReadOnly();

        public IReadOnlyList<ICommandLineCommand> Commands => m_commands.AsReadOnly();

        public string Name { get; protected set; }
        public string Description { get; protected set; }
        public bool IsRequired { get; protected set; }
        public bool AutoExecute { get; protected set; } = true;

        public abstract void Execute();

        public abstract ICommandParserResult Parse(IArgumentManager argumentManager);
    }
}
