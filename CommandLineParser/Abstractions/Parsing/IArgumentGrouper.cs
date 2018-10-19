using MatthiWare.CommandLine.Abstractions.Command;
using MatthiWare.CommandLine.Abstractions.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MatthiWare.CommandLine.Abstractions.Parsing
{
    public interface IArgumentGrouper
    {
        bool TryGetValue(ICommandLineOption cmd, out string value);
    }
}
