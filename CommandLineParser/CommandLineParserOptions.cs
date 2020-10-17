using System.Diagnostics;

namespace MatthiWare.CommandLine
{
    /// <summary>
    /// Configuration options for <see cref="CommandLineParser"></see>
    /// </summary>
    public class CommandLineParserOptions
    {
        /// <summary>
        /// Prefix for the short option
        /// </summary>
        public string PrefixShortOption { get; set; } = "-";

        /// <summary>
        /// Prefix for the long option
        /// </summary>
        public string PrefixLongOption { get; set; } = "--";

        /// <summary>
        /// Postfix for the long option
        /// </summary>
        public string PostfixOption { get; set; } = "=";

        /// <summary>
        /// Stops parsing of remaining arguments after this has been found
        /// </summary>
        public string StopParsingAfter { get; set; }

        /// <summary>
        /// Help option name. 
        /// Accepts both formatted and unformatted help name. 
        /// If the name is a single string it will use the <see cref="HelpOptionName"/>
        /// If the name is split for example h|help it will use the following format <![CDATA[<shortOption>|<longOption>]]>
        /// </summary>
        public string HelpOptionName { get; set; } = "h|help";

        /// <summary>
        /// Enable or disable the help option
        /// <see cref="HelpOptionName"/>
        /// </summary>
        public bool EnableHelpOption { get; set; } = true;

        /// <summary>
        /// Enables or disables the automatic usage and error printing
        /// </summary>
        public bool AutoPrintUsageAndErrors { get; set; } = true;

        /// <summary>
        /// Sets the application name. Will use the <see cref="Process.ProcessName"/> by default if none is specified.
        /// </summary>
        public string AppName { get; set; }

        internal (string shortOption, string longOption) GetConfiguredHelpOption()
        {
            var tokens = HelpOptionName.Split('|');

            string shortResult;
            string longResult = null;

            if (tokens.Length > 1)
            {
                shortResult = $"{PrefixShortOption}{tokens[0]}";
                longResult = $"{PrefixLongOption}{tokens[1]}";
            }
            else
            {
                shortResult = $"{PrefixLongOption}{tokens[0]}";
            }

            return (shortResult, longResult);
        }
    }
}
