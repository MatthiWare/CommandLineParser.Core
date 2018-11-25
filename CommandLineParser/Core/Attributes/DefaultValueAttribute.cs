namespace MatthiWare.CommandLine.Core.Attributes
{
    public class DefaultValueAttribute : BaseAttribute
    {
        public object DefaultValue { get; private set; }

        public DefaultValueAttribute(object defaultValue) => DefaultValue = defaultValue;
    }
}
