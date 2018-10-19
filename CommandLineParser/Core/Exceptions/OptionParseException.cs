using System;
using System.Collections.Generic;
using System.Text;
using MatthiWare.CommandLine.Abstractions;
using MatthiWare.CommandLine.Abstractions.Models;

namespace MatthiWare.CommandLine.Core.Exceptions
{
    public class OptionParseException : Exception
    {
        private ICommandLineOption option;
        private ArgumentModel argModel;

        public OptionParseException(ICommandLineOption option, ArgumentModel argModel)
            : base($"Cannot parse option '{argModel.Key}:{argModel.Value ?? "NULL"}'.")
        {
            this.option = option;
            this.argModel = argModel;
        }
    }
}
