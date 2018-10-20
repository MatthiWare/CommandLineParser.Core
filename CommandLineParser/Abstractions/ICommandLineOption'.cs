using System;
using System.Collections.Generic;
using System.Text;

namespace MatthiWare.CommandLine.Abstractions
{
    public interface ICommandLineOption<TProperty> : ICommandLineOption
    {

        TProperty DefaultValue { get; set; }

    }
}
