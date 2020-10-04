namespace MatthiWare.CommandLine.Abstractions.Parsing
{
    public readonly struct UnusedArgumentModel
    {
        public IArgument Argument { get; }
        public string Key { get;  }

        public UnusedArgumentModel(string key, IArgument argument)
        {
            Key = key;
            Argument = argument;
        }
    }
}
