using MatthiWare.CommandLine.Abstractions.Command;
using System.Collections.Generic;

namespace MatthiWare.CommandLine.Core.Exceptions
{
    public class CommandNotFoundException : KeyNotFoundException
    {
        public ICommandLineCommand Command { get; private set; }

        public CommandNotFoundException(ICommandLineCommand cmd)
            :base($"Required command '{cmd.HasShortName}' or '{cmd.LongName}' not found!")
        {
            Command = cmd;
        }
    }
}
