using System;
using System.Collections.Generic;
using System.Text;
using MatthiWare.CommandLine.Abstractions;
using MatthiWare.CommandLine.Abstractions.Command;
using MatthiWare.CommandLine.Abstractions.Parsing.Command;

namespace MatthiWare.CommandLine.Core.Command
{
    internal abstract class CommandLineCommandBase :
        ICommandLineCommandParser,
        ICommandLineCommand
    {
        protected readonly List<ICommandLineArgumentOption> options = new List<ICommandLineArgumentOption>();

        public IReadOnlyList<ICommandLineArgumentOption> Options => options.AsReadOnly();

        public string ShortName { get; protected set; }
        public string LongName { get; protected set; }
        public string HelpText { get; protected set; }
        public bool IsRequired { get; protected set; }
        public bool HasShortName => ShortName != null;
        public bool HasLongName => LongName != null;

        public abstract void Execute();

        public abstract ICommandParserResult Parse(List<string> args, int startIndex);
    }
}
