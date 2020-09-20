namespace MatthiWare.CommandLine.Core.Attributes
{
    /// <summary>
    /// Specified if the command/option is required
    /// </summary>
    public sealed class RequiredAttribute : BaseAttribute
    {
        /// <summary>
        /// Is it required?
        /// </summary>
        public bool Required { get; private set; }

        /// <summary>
        /// Specifies if the command/option is required
        /// </summary>
        /// <param name="required">True if required, false if not</param>
        public RequiredAttribute(bool required = true) => Required = required;
    }
}
