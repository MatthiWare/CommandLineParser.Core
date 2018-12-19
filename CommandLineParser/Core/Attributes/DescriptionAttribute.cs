namespace MatthiWare.CommandLine.Core.Attributes
{
    public class DescriptionAttribute : BaseAttribute
    {
        public string Description { get; private set; }

        public DescriptionAttribute(string helpText) => Description = helpText;
    }
}
