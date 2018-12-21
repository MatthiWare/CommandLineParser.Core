namespace MatthiWare.CommandLine.Abstractions
{
    /// <summary>
    /// Option configuration options
    /// </summary>
    public interface ICommandLineOption : IArgument
    {
        string ShortName { get; }
        string LongName { get; }
        string Description { get; }
        bool IsRequired { get; }
        bool HasShortName { get; }
        bool HasLongName { get; }
        bool HasDefault { get; }
    }
}
