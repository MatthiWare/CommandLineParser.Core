using MatthiWare.CommandLine.Abstractions;
using MatthiWare.CommandLine.Abstractions.Command;
using MatthiWare.CommandLine.Abstractions.Models;
using MatthiWare.CommandLine.Core.Command;
using System;
using System.Collections.Generic;
using System.Text;

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
