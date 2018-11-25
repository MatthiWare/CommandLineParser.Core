namespace MatthiWare.CommandLine.Core.Attributes
{
    public class NameAttribute : BaseAttribute
    {
        public string ShortName { get; private set; }
        public string LongName { get; private set; }

        public NameAttribute(string shortName)
            : this(shortName, string.Empty)
        { }

        public NameAttribute(string shortName, string longName)
        {
            ShortName = shortName;
            LongName = longName;
        }
    }
}
