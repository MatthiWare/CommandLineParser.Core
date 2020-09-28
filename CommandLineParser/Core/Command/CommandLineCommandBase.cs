using MatthiWare.CommandLine.Abstractions;
using MatthiWare.CommandLine.Abstractions.Command;
using MatthiWare.CommandLine.Abstractions.Parsing;
using MatthiWare.CommandLine.Abstractions.Parsing.Command;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace MatthiWare.CommandLine.Core.Command
{
    [DebuggerDisplay("Command [{Name}] Options: {m_options.Count} Commands: {m_commands.Count} Required: {IsRequired} Execute: {AutoExecute}")]
    internal abstract class CommandLineCommandBase :
        ICommandLineCommandParser,
        ICommandLineCommandContainer,
        ICommandLineCommand
    {
        protected readonly Dictionary<string, CommandLineOptionBase> m_options = new Dictionary<string, CommandLineOptionBase>();
        protected readonly List<CommandLineCommandBase> m_commands = new List<CommandLineCommandBase>();

        public IReadOnlyList<ICommandLineOption> Options => new ReadOnlyCollectionWrapper<string, CommandLineOptionBase>(m_options.Values);

        public IReadOnlyList<ICommandLineCommand> Commands => m_commands.AsReadOnly();

        public string Name { get; protected set; }
        public string Description { get; protected set; }
        public bool IsRequired { get; protected set; }
        public bool AutoExecute { get; protected set; } = true;
        
        public abstract Task ExecuteAsync(CancellationToken cancellationToken);

        public abstract Task<ICommandParserResult> ParseAsync(CancellationToken cancellationToken);
    }
}
