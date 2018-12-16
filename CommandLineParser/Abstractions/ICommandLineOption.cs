using MatthiWare.CommandLine.Abstractions.Command;

namespace MatthiWare.CommandLine.Abstractions
{
    /// <summary>
    /// Option configuration options
    /// </summary>
    public interface ICommandLineOption
    {
        string ShortName { get; }
        string LongName { get; }
        string HelpText { get; }
        bool IsRequired { get; }
        bool HasShortName { get; }
        bool HasLongName { get; }
        bool HasDefault { get; }
    }
}
