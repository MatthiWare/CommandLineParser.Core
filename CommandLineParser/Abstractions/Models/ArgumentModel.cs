using System.Diagnostics;

namespace MatthiWare.CommandLine.Abstractions.Models
{
    /// <summary>
    /// Model for command line arguments
    /// </summary>
    [DebuggerDisplay("Argument key: {Key} value: {Value}")]
    public struct ArgumentModel
    {
        /// <summary>
        /// Argument identifier
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Value of the argument
        /// Can be null
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Checks if an value has been provided in the model
        /// </summary>
        public bool HasValue => Value != null;

        /// <summary>
        /// Creates a new instance of the argument model
        /// </summary>
        /// <param name="key">model identifier</param>
        /// <param name="value">model value</param>
        public ArgumentModel(string key, string value)
        {
            this.Key = key;
            this.Value = value;
        }
    }
}
