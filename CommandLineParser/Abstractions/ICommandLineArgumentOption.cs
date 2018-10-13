using System;
using System.Collections.Generic;
using System.Text;

namespace MatthiWare.CommandLine.Abstractions
{
    public interface ICommandLineArgumentOption
    {
        string ShortName { get; }
        string LongName { get; }
        string HelpText { get; }
        bool IsRequired { get; }
        bool HasDefault { get; }
        bool HasShortName { get; }
        bool HasLongName { get; }
    }
}
