using MatthiWare.CommandLine.Abstractions;
using MatthiWare.CommandLine.Abstractions.Models;

namespace MatthiWare.CommandLine.Core.Exceptions
{
    /// <summary>
    /// Indicates that an option was unable to be parsed
    /// This could be caused by an missing <see cref="Abstractions.Parsing.ICommandLineArgumentResolver"/>.
    /// </summary>
    public class OptionParseException : BaseParserException
    {
        /// <summary>
        /// The option that failed
        /// </summary>
        public ICommandLineOption Option => (ICommandLineOption)Argument;

        /// <summary>
        /// Provided argument model
        /// </summary>
        public ArgumentModel ArgumentModel { get; }

        /// <summary>
        /// Creates a new <see cref="OptionParseException"/>
        /// </summary>
        /// <param name="option">The failed option</param>
        /// <param name="argModel">The specified argument</param>
        public OptionParseException(ICommandLineOption option, ArgumentModel argModel)
            : base(option, CreateMessage(option, argModel))
        {
            ArgumentModel = argModel;
        }

        private static string CreateMessage(ICommandLineOption option, ArgumentModel argModel)
        {
            bool hasShort = option.HasShortName;
            bool hasLong = option.HasLongName;
            bool hasBoth = hasShort && hasLong;

            string optionName = !hasShort && !hasLong ? option.ToString() : string.Empty;
            string hasBothSeperator = hasBoth ? "|" : string.Empty;
            string shortName = hasShort ? option.ShortName : string.Empty;
            string longName = hasLong ? option.LongName : string.Empty;

            return $"Unable to parse option '{shortName}{hasBothSeperator}{longName}{optionName}' value '{string.Join(", ", argModel.Values)}' is invalid!";
        }
    }
}
