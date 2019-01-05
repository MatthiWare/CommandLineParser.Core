using System;
using System.Collections.Generic;
using System.Linq;

using MatthiWare.CommandLine.Abstractions.Command;
using MatthiWare.CommandLine.Abstractions.Parsing.Command;

namespace MatthiWare.CommandLine.Core.Command
{
    internal class CommandParserResult : ICommandParserResult
    {
        private readonly CommandLineCommandBase m_cmd;
        private readonly List<ICommandParserResult> commandParserResults = new List<ICommandParserResult>();

        public bool HasErrors { get; private set; } = false;

        public Exception Error { get; private set; } = null;

        public ICommandLineCommand Command => m_cmd;

        public IReadOnlyCollection<ICommandParserResult> SubCommands => commandParserResults;

        public bool HelpRequested { get; set; } = false;

        public CommandParserResult(CommandLineCommandBase command)
        {
            m_cmd = command;
        }

        public void MergeResult(ICollection<Exception> errors)
        {
            HasErrors = errors.Any();

            if (!HasErrors) return;

            Error = (errors.Count > 1) ?
                new AggregateException(errors) :
                errors.First();
        }

        public void MergeResult(ICommandParserResult result)
        {
            HasErrors |= result.HasErrors;

            commandParserResults.Add(result);
        }

        public void ExecuteCommand() => m_cmd.Execute();
    }
}
