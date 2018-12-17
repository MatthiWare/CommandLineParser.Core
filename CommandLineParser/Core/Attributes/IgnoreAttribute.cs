namespace MatthiWare.CommandLine.Core.Attributes
{
    /// <summary>
    /// Specifies if the option should be ignored.
    /// </summary>
    public class IgnoreAttribute : BaseAttribute
    {
        /// <summary>
        /// Indicates if the option should be ignored
        /// </summary>
        public bool IgnoreOption { get; private set; }

        /// <summary>
        /// Specifies if the option should be ignored.
        /// </summary>
        /// <param name="ignore">Ignore the option, true or false</param>
        public IgnoreAttribute(bool ignore = true)
        {
            IgnoreOption = ignore;
        }
    }
}
