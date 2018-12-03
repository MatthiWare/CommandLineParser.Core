using System;
using MatthiWare.CommandLine.Abstractions.Command;

namespace MatthiWare.CommandLine.Core.Exceptions
{
    /// <summary>
    /// Unable to parse the command
    /// </summary>
    public class CommandParseException : Exception
    {
        /// <summary>
        /// Command that caused the parsing error
        /// </summary>
        public ICommandLineCommand Option { get; set; }

        public CommandParseException(ICommandLineCommand option, Exception innerException)
            : base("", innerException)
        {
            Option = option;
        }
    }
}
