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
            : base($"Required argument '{option.HasShortName}' or '{option.LongName}' not found!")
        { }

        private string CreateMessage()
        {
            bool hasShortName = Option.HasShortName;
            bool hasLongName = Option.HasLongName;
            bool hasBoth = hasShortName && hasLongName;

            string shortName = hasShortName ? $"'{Option.ShortName}'" : string.Empty;
            string longName = hasLongName ? $"'{Option.LongName}'" : string.Empty;
            string or = hasBoth ? " or " : string.Empty;

            return $"Required argument {shortName}{or}{longName} not found!";
        }
    }
}
