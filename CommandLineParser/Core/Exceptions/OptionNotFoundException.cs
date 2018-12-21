using System.Collections.Generic;

using MatthiWare.CommandLine.Abstractions;

namespace MatthiWare.CommandLine.Core.Exceptions
{
    /// <summary>
    /// Indiciates the configured required option is not found
    /// </summary>
    public class OptionNotFoundException : KeyNotFoundException
    {
        /// <summary>
        /// Option that was not found
        /// </summary>
        public ICommandLineOption Option { get; private set; }

        public OptionNotFoundException(ICommandLineOption option)
            : base(CreateMessage(option))
        { }

        private static string CreateMessage(ICommandLineOption option)
        {
            bool hasShortName = option.HasShortName;
            bool hasLongName = option.HasLongName;
            bool hasBoth = hasShortName && hasLongName;

            string shortName = hasShortName ? $"'{option.ShortName}'" : string.Empty;
            string longName = hasLongName ? $"'{option.LongName}'" : string.Empty;
            string or = hasBoth ? " or " : string.Empty;

            return $"Required argument {shortName}{or}{longName} not found!";
        }
    }
}
