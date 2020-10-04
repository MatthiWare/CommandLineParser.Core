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
            var hasShort = option.HasShortName;
            var hasLong = option.HasLongName;
            var hasBoth = hasShort && hasLong;

            var hasBothSeperator = hasBoth ? "|" : string.Empty;
            var shortName = hasShort ? $"{option.ShortName}" : string.Empty;
            var longName = hasLong ? $"{option.LongName}" : string.Empty;
            var optionString = hasShort || hasLong ? string.Empty : option.ToString();

            return $"Required option '{shortName}{hasBothSeperator}{longName}{optionString}' not found!";
        }
    }
}
