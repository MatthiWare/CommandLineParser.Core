using System.Diagnostics;

namespace MatthiWare.CommandLine
{
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
        /// Postfix for the short option
        /// </summary>
        public string PostfixShortOption { get; set; } = string.Empty;

        /// <summary>
        /// Postfix for the long option
        /// </summary>
        public string PostfixLongOption { get; set; } = string.Empty;

        /// <summary>
        /// Help option name. 
        /// Accepts both formatted and unformatted help name. 
        /// If the name is a single string it will use the <see cref="PrefixLongOption"/>
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
    }
}
