using System;
using MatthiWare.CommandLine.Abstractions;
using MatthiWare.CommandLine.Abstractions.Models;

namespace MatthiWare.CommandLine.Core.Exceptions
{
    /// <summary>
    /// Indicates that an option was unable to be parsed
    /// This could be caused by an missing <see cref="MatthiWare.CommandLine.Abstractions.Parsing.ICommandLineArgumentResolver"/>.
    /// </summary>
    public class OptionParseException : Exception
    {
        private readonly ICommandLineOption option;
        private ArgumentModel argModel;

        public OptionParseException(ICommandLineOption option, ArgumentModel argModel)
            : base($"Cannot parse option '{argModel.Key}:{argModel.Value ?? "NULL"}'.")
        {
            this.option = option;
            this.argModel = argModel;
        }
    }
}
