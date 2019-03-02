using MatthiWare.CommandLine.Abstractions;

namespace MatthiWare.CommandLine.Core.Exceptions
{
    /// <summary>
    /// Indiciates the configured required option is not found
    /// </summary>
    public class OptionNotFoundException : BaseParserException
    {
        /// <summary>
        /// Option that was not found
        /// </summary>
        public ICommandLineOption Option => (ICommandLineOption)Argument;

        /// <summary>
        /// Creates a new <see cref="OptionNotFoundException"/> for a given <see cref="ICommandLineOption"/>
        /// </summary>
        /// <param name="option">The option that was not found</param>
        public OptionNotFoundException(ICommandLineOption option)
            : base(option, CreateMessage(option))
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
