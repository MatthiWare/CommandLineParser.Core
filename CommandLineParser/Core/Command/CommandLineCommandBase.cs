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
        ICommandLineCommand,
        IUsageDisplay
    {
        protected readonly List<CommandLineOptionBase> m_options = new List<CommandLineOptionBase>();

        public IReadOnlyList<ICommandLineOption> Options => m_options.AsReadOnly();

        public string Name { get; protected set; }
        public string Description { get; protected set; }
        public bool IsRequired { get; protected set; }
        public bool AutoExecute { get; protected set; } = true;

        public abstract void Execute();

        public abstract ICommandParserResult Parse(IArgumentManager argumentManager);

        public string ToShortUsage()
            => $"  {Name}\t\t{Description}";

        public string ToUsage()
        {
            var sb = new StringBuilder();

            sb.AppendLine(Description);

            if (m_options.Count == 0)
                return sb.ToString();

            sb.AppendLine("Options: ");

            foreach (var opt in m_options)
                sb.AppendLine(opt.ToUsage());

            return sb.ToString();
        }
    }
}
