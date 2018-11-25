using MatthiWare.CommandLine.Abstractions;
using System.Collections.Generic;

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
