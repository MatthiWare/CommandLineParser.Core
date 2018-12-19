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
    }
}
