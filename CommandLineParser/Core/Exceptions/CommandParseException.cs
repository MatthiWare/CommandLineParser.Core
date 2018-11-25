using MatthiWare.CommandLine.Abstractions.Command;
using System;

namespace MatthiWare.CommandLine.Core.Exceptions
{
    public class CommandParseException : Exception
    {
        public ICommandLineCommand Option { get; set; }

        public CommandParseException(ICommandLineCommand option, Exception innerException)
            : base("", innerException)
        {
            Option = option;
        }
    }
}
