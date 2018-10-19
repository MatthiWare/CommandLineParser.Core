using MatthiWare.CommandLine.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace MatthiWare.CommandLine.Core.Exceptions
{
    public class OptionNotFoundException : KeyNotFoundException
    {
        public ICommandLineOption Option { get; private set; }

        public OptionNotFoundException(ICommandLineOption option) 
            : base($"Required argument '{option.HasShortName}' or '{option.LongName}' not found!")
        {

        }
    }
}
