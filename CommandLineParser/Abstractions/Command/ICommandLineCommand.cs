using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace MatthiWare.CommandLine.Abstractions.Command
{
    public interface ICommandLineCommand
    {
        string ShortName { get; set; }
        string LongName { get; set; }
        string HelpText { get; set; }
        bool IsRequired { get; set; }
        bool HasShortName { get; }
        bool HasLongName { get; }
        void Execute();
    }
}
