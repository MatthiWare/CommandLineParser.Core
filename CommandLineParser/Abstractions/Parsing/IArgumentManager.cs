using System;
using System.Collections.Generic;
using System.Text;
using MatthiWare.CommandLine.Abstractions.Command;
using MatthiWare.CommandLine.Abstractions.Models;

namespace MatthiWare.CommandLine.Abstractions.Parsing
{
    public interface IArgumentManager
    {
        bool TryGetValue(ICommandLineCommand cmd, out ArgumentModel model);
    }
}
