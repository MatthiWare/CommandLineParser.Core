using System;
using System.Collections.Generic;
using System.Linq;
using MatthiWare.CommandLine.Abstractions.Parsing.Command;

namespace MatthiWare.CommandLine.Core.Command
{
    internal class CommandParserResult : ICommandParserResult
    {
        private readonly CommandLineCommandBase command;

        public bool HasErrors { get; private set; }

        public Exception Error { get; private set; } = null;

        public CommandParserResult(CommandLineCommandBase command) => this.command = command;

        public void MergeResult(ICollection<Exception> errors)
        {
            HasErrors = errors.Any();

            if (!HasErrors) return;

            Error = (errors.Count > 1) ?
                new AggregateException(errors) :
                errors.First();
        }

        public void ExecuteCommand()
        {
            if (HasErrors) throw new InvalidOperationException("Command contains errors");

            command.Execute();
        }
    }
}
