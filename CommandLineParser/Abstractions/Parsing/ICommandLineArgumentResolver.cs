using System;
using System.Collections.Generic;
using System.Text;
using MatthiWare.CommandLine.Core.Parsing;

namespace MatthiWare.CommandLine.Abstractions.Parsing
{
    public interface ICommandLineArgumentResolver<T>
    {

        bool CanResolve(OptionModel model);

        T Resolve(OptionModel model);

    }
}
