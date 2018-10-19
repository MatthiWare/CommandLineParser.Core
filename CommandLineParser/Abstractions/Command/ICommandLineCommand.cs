using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace MatthiWare.CommandLine.Abstractions.Command
{
    public interface ICommandLineCommand
    {
        string ShortName { get; }
        string LongName { get; }
        string HelpText { get; }
        bool IsRequired { get; }
        bool HasShortName { get; }
        bool HasLongName { get; }
    }
}
