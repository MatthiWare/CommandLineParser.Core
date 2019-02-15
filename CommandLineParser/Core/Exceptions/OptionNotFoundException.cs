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

        /// <summary>
        /// Creates a new <see cref="OptionNotFoundException"/> for a given <see cref="ICommandLineOption"/>
        /// </summary>
        /// <param name="option">The option that was not found</param>
        public OptionNotFoundException(ICommandLineOption option)
            : base(CreateMessage(option))
        { }

        private static string CreateMessage(ICommandLineOption option)
        {
            bool hasShort = option.HasShortName;
            bool hasLong = option.HasLongName;
            bool hasBoth = hasShort && hasLong;

            string hasBothSeperator = hasBoth ? "|" : string.Empty;
            string shortName = hasShort ? $"{option.ShortName}" : string.Empty;
            string longName = hasLong ? $"{option.LongName}" : string.Empty;

            return $"Required option '{shortName}{hasBothSeperator}{longName}' not found!";
        }
    }
}
