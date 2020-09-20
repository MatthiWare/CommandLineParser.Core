namespace MatthiWare.CommandLine.Core.Attributes
{
    /// <summary>
    /// Specifies the description of the options
    /// </summary>
    public sealed class DescriptionAttribute : BaseAttribute
    {
        /// <summary>
        /// Description
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Specifies the description of the options
        /// </summary>
        /// <param name="description">description text</param>
        public DescriptionAttribute(string description) => Description = description;
    }
}
