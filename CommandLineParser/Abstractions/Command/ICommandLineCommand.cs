﻿namespace MatthiWare.CommandLine.Abstractions.Command
{
    /// <summary>
    /// Command configuration options
    /// </summary>
    public interface ICommandLineCommand
    {
        string ShortName { get; }
        string LongName { get; }
        string HelpText { get; }
        bool IsRequired { get; }
        bool HasShortName { get; }
        bool HasLongName { get; }
        bool HasDefault { get; }
        bool AutoExecute { get; }
    }
}
