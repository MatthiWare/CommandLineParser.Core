using MatthiWare.CommandLine.Abstractions;
using MatthiWare.CommandLine.Abstractions.Command;
using MatthiWare.CommandLine.Abstractions.Parsing.Command;
using MatthiWare.CommandLine.Core.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MatthiWare.CommandLine.Core.Parsing.Command
{
    internal class CommandParserResult : ICommandParserResult
    {
        private readonly CommandLineCommandBase m_cmd;
        private readonly List<ICommandParserResult> commandParserResults = new List<ICommandParserResult>();
        private readonly List<Exception> exceptions = new List<Exception>();

        public bool HasErrors { get; private set; } = false;

        public ICommandLineCommand Command => m_cmd;

        public IReadOnlyCollection<ICommandParserResult> SubCommands => commandParserResults;

        public bool HelpRequested => HelpRequestedFor != null;

        public IArgument HelpRequestedFor { get; set; } = null;

        public IReadOnlyCollection<Exception> Errors => exceptions;

        public bool Found => true;

        public bool Executed { get; private set; } = false;

        public CommandParserResult(CommandLineCommandBase command)
        {
            m_cmd = command;
        }

        public void MergeResult(ICollection<Exception> errors)
        {
            HasErrors = errors.Any();

            if (!HasErrors)
            {
                return;
            }

            exceptions.AddRange(errors);
        }

        public void MergeResult(ICommandParserResult result)
        {
            HasErrors |= result.HasErrors;

            if (result.HelpRequested)
            {
                HelpRequestedFor = result.HelpRequestedFor;
            }

            commandParserResults.Add(result);
        }

        public async Task ExecuteCommandAsync(CancellationToken cancellationToken)
        {
            await m_cmd.ExecuteAsync(cancellationToken);
            Executed = true;
        }
    }
}
