using System;
using System.Collections.Generic;
using System.Text;

namespace MatthiWare.CommandLine.Abstractions.Parsing
{
    public interface IParserResult<TResult>
    {
        TResult Result { get; }

        bool HasErrors { get; }

        Exception Error { get; }
    }
}
