using System.Collections.Generic;
using MatthiWare.CommandLine.Abstractions;
using MatthiWare.CommandLine.Abstractions.Command;
using MatthiWare.CommandLine.Abstractions.Parsing;
using MatthiWare.CommandLine.Abstractions.Parsing.Command;

namespace MatthiWare.CommandLine.Core.Command
{
    internal abstract class CommandLineCommandBase :
        ICommandLineCommandParser,
        ICommandLineCommand
    {
        protected readonly List<CommandLineOptionBase> m_options = new List<CommandLineOptionBase>();

        public IReadOnlyList<ICommandLineOption> Options => m_options.AsReadOnly();

        public string ShortName { get; protected set; }
        public string LongName { get; protected set; }
        public string HelpText { get; protected set; }
        public bool IsRequired { get; protected set; }
        public bool HasShortName => ShortName != null;
        public bool HasLongName => LongName != null;
        public bool HasDefault => false;

        public abstract void Execute();

        public abstract ICommandParserResult Parse(IArgumentManager argumentManager);
    }
}
