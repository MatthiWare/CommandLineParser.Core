using System;
using System.Collections.Generic;
using System.Text;
using MatthiWare.CommandLine.Abstractions.Models;

namespace MatthiWare.CommandLine.Abstractions.Parsing
{
    public interface IParser
    {
        void Parse(ArgumentModel model);
    }
}
