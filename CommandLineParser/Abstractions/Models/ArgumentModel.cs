using System.Collections.Generic;
using System.Diagnostics;

namespace MatthiWare.CommandLine.Abstractions.Models
{
    /// <summary>
    /// Model for command line arguments
    /// </summary>
    [DebuggerDisplay("Argument key: {Key} values({Values.Count}): {string.Join(\", \", Values)}")]
    public class ArgumentModel
    {
        /// <summary>
        /// Argument identifier
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Value of the argument
        /// </summary>
        public List<string> Values { get; } = new List<string>();

        /// <summary>
        /// Checks if an value has been provided in the model
        /// </summary>
        public bool HasValue => Values.Count > 0;

        public ArgumentModel(string key)
        {
            this.Key = key;
        }

        /// <summary>
        /// Creates a new instance of the argument model
        /// </summary>
        /// <param name="key">model identifier</param>
        /// <param name="value">model value</param>
        public ArgumentModel(string key, string value)
        {
            this.Key = key;

            if (!string.IsNullOrEmpty(value))
            { 
                Values.Add(value);
            }
        }
    }
}
