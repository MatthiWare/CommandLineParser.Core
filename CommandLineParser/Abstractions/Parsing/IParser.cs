using MatthiWare.CommandLine.Abstractions.Models;

namespace MatthiWare.CommandLine.Abstractions.Parsing
{
    /// <summary>
    /// API for parsing arguments
    /// </summary>
    public interface IParser
    {
        /// <summary>
        /// Checks if the argument can be parsed
        /// </summary>
        /// <param name="model"><see cref="ArgumentModel"/></param>
        /// <returns>True if the arguments can be parsed, false if not.</returns>
        bool CanParse(ArgumentModel model);

        /// <summary>
        /// Parses the model
        /// Check <see cref="CanParse(ArgumentModel)"/> to see if this method will succeed.
        /// </summary>
        /// <param name="model"></param>
        void Parse(ArgumentModel model);
    }
}
