namespace MatthiWare.CommandLine
{
    public class CommandLineParserOptions
    {
        public string PrefixShortOption { get; set; } = "-";
        public string PrefixLongOption { get; set; } = "--";
        /// <summary>
        /// Help option name. 
        /// Accepts both formatted and unformatted help name. 
        /// If the name is a single string it will use the <see cref="PrefixLongOption"/>
        /// If the name is split for example h|help it will use the following format <![CDATA[<shortOption>|<longOption>]]>
        /// </summary>
        public string HelpOptionName { get; set; } = "h|help";
        public bool EnableHelpOption { get; set; } = true;
        public bool AutoPrintUsageAndErrors { get; set; } = true;
        public string AppName { get; set; }
    }
}
