namespace MatthiWare.CommandLine.Core.Attributes
{
    public class RequiredAttribute : BaseAttribute
    {
        public bool Required { get; private set; }

        public RequiredAttribute(bool required = true) => Required = required;
    }
}
