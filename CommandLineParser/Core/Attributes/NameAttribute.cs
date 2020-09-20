namespace MatthiWare.CommandLine.Core.Attributes
{
    /// <summary>
    /// Specifies the name of the option/command
    /// </summary>
    public sealed class NameAttribute : BaseAttribute
    {
        /// <summary>
        /// Short version
        /// </summary>
        public string ShortName { get; private set; }

        /// <summary>
        /// Long version
        /// </summary>
        public string LongName { get; private set; }

        /// <summary>
        /// Specifies the name
        /// </summary>
        /// <param name="shortName">short name</param>
        public NameAttribute(string shortName)
            : this(shortName, string.Empty)
        { }

        /// <summary>
        /// Specified the name
        /// </summary>
        /// <param name="shortName">short name</param>
        /// <param name="longName">long name</param>
        public NameAttribute(string shortName, string longName)
        {
            ShortName = shortName;
            LongName = longName;
        }
    }
}
