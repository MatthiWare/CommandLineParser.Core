using System;

using MatthiWare.CommandLine.Abstractions;
using MatthiWare.CommandLine.Abstractions.Models;

namespace MatthiWare.CommandLine.Core.Exceptions
{
    /// <summary>
    /// Indicates that an option was unable to be parsed
    /// This could be caused by an missing <see cref="Abstractions.Parsing.ICommandLineArgumentResolver"/>.
    /// </summary>
    public class OptionParseException : Exception
    {
        private readonly ICommandLineOption option;
        private ArgumentModel argModel;

        public OptionParseException(ICommandLineOption option, ArgumentModel argModel)
            : base(CreateMessage(option, argModel))
        {
            this.option = option;
            this.argModel = argModel;
        }

        private static string CreateMessage(ICommandLineOption option, ArgumentModel argModel)
        {
            bool hasShort = option.HasShortName;
            bool hasLong = option.HasLongName;
            bool hasBoth = hasShort && hasLong;

            string hasBothSeperator = hasBoth ? "|" : string.Empty;
            string shortName = hasShort ? option.ShortName : string.Empty;
            string longName = hasLong ? option.LongName : string.Empty;

            return $"Unable to parse option '{shortName}{hasBothSeperator}{longName}' value '{argModel.Value}' is invalid!";
        }
    }
}
