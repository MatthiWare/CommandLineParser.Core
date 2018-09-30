using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MatthiWare.CommandLine.Abstractions.Models;
using MatthiWare.CommandLine.Abstractions.Parsing;

namespace MatthiWare.CommandLine.Core.Parsing.Resolvers
{
    internal class StringResolver : ICommandLineArgumentResolver<string>
    {
        public bool CanResolve(ArgumentModel model) => true;

        public string Resolve(ArgumentModel model)
            => model.HasValue ? model.Value : null;
    }
}
