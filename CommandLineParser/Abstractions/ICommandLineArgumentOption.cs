using System;
using System.Collections.Generic;
using System.Text;

namespace MatthiWare.CommandLine.Abstractions
{
    public interface ICommandLineArgumentOption
    {
        string ShortName { get; set; }
        string LongName { get; set; }
        string HelpText { get; set; }
        bool IsRequired { get; set; }
        bool HasDefault { get; }
    }
}
