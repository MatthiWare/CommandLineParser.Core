using MatthiWare.CommandLine.Abstractions;

namespace MatthiWare.CommandLine.Core
{
    internal class CommandLineArgumentOption<TProperty> : ICommandLineArgumentOption<TProperty>, ICommandLineArgumentOption
    {
        private TProperty m_defaultValue = default(TProperty);

        public TProperty DefaultValue
        {
            get => m_defaultValue;
            set
            {
                HasDefault = true;
                m_defaultValue = value;
            }
        }

        public string ShortName { get; set; }
        public string LongName { get; set; }
        public string HelpText { get; set; }
        public bool IsRequired { get; set; }
        public bool HasDefault { get; private set; }
    }
}