namespace MatthiWare.CommandLine.Core.Attributes
{
    /// <summary>
    /// Specifies the default value of the property
    /// </summary>
    public class DefaultValueAttribute : BaseAttribute
    {
        /// <summary>
        /// Default value
        /// </summary>
        public object DefaultValue { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="defaultValue">default value</param>
        public DefaultValueAttribute(object defaultValue) => DefaultValue = defaultValue;
    }
}
