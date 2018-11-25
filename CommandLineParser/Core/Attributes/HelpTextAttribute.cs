namespace MatthiWare.CommandLine.Core.Attributes
{
    public class HelpTextAttribute : BaseAttribute
    {
        public string HelpText { get; private set; }

        public HelpTextAttribute(string helpText) => HelpText = helpText;
    }
}
