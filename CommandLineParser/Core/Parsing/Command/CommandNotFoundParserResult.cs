using MatthiWare.CommandLine.Abstractions;
using MatthiWare.CommandLine.Abstractions.Command;
using MatthiWare.CommandLine.Abstractions.Parsing.Command;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MatthiWare.CommandLine.Core.Parsing.Command
{
    internal class CommandNotFoundParserResult : ICommandParserResult
    {
        public bool Found => false;

        public IArgument HelpRequestedFor => null;

        public bool HelpRequested => false;

        public IReadOnlyCollection<ICommandParserResult> SubCommands => new List<ICommandParserResult>();

        public ICommandLineCommand Command { get; }

        public bool HasErrors => false;

        public IReadOnlyCollection<Exception> Errors => new List<Exception>();

        public bool Executed => false;

        public void ExecuteCommand() { }

        public Task ExecuteCommandAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public CommandNotFoundParserResult(ICommandLineCommand cmd) => Command = cmd;
    }
}
