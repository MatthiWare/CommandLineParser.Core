namespace MatthiWare.CommandLine
{
    public class CommandLineParserOptions
    {
        public string PrefixShortOption { get; set; } = "-";
        public string PrefixLongOption { get; set; } = "--";
        public string HelpOptionName { get; set; } = "help";
        public bool EnableHelpOption { get; set; } = true;
        public string AppName { get; set; }
    }
}
