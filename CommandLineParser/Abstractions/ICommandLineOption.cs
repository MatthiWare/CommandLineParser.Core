namespace MatthiWare.CommandLine.Abstractions
{
    /// <summary>
    /// Option configuration options
    /// </summary>
    public interface ICommandLineOption : IArgument
    {
        /// <summary>
        /// Short name of the option
        /// </summary>
        string ShortName { get; }

        /// <summary>
        /// Long name of the option
        /// </summary>
        string LongName { get; }

        /// <summary>
        /// Description of the option
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Indicates if the option is required
        /// </summary>
        bool IsRequired { get; }

        /// <summary>
        /// Indicates if a short option name has been specified
        /// </summary>
        bool HasShortName { get; }

        /// <summary>
        /// Indicates if a long option name has been specified
        /// </summary>
        bool HasLongName { get; }

        /// <summary>
        /// Inidicates if a default value has been specified for this option
        /// </summary>
        bool HasDefault { get; }

        /// <summary>
        /// Option order
        /// </summary>
        int? Order { get; }

        /// <summary>
        /// Can have multiple values?
        /// </summary>
        bool AllowMultipleValues { get; }
    }
}
