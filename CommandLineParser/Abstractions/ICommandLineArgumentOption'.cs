using System;
using System.Collections.Generic;
using System.Text;

namespace MatthiWare.CommandLine.Abstractions
{
    public interface ICommandLineArgumentOption<TProperty> : ICommandLineArgumentOption
    {

        TProperty DefaultValue { get; set; }

    }
}
