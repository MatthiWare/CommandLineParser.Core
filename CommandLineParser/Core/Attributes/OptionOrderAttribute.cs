namespace MatthiWare.CommandLine.Core.Attributes
{
    /// <summary>
    /// Give an order to options
    /// </summary>
    /// <example>app.exe move "first/argument/path" "second/argument/path"</example>
    public sealed class OptionOrderAttribute : BaseAttribute
    {
        /// <summary>
        /// Order in which the options will be parsed (Ascending). 
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Assign an order to options
        /// </summary>
        /// <param name="order">Order in which options will be parsed (Ascending)</param>
        public OptionOrderAttribute(int order)
        {
            Order = order;
        }
    }
}
