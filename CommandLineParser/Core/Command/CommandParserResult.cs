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

        public bool HasErrors { get; private set; }

        public Exception Error { get; private set; } = null;

        public ICommandLineCommand Command => m_cmd;

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

        public void ExecuteCommand() => m_cmd.Execute();
    }
}
