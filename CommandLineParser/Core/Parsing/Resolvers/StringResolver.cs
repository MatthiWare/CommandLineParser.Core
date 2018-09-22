using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MatthiWare.CommandLine.Abstractions.Parsing;

namespace MatthiWare.CommandLine.Core.Parsing.Resolvers
{
    internal class StringResolver : ICommandLineArgumentResolver<string>
    {
        public bool CanResolve(OptionModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Value) || !model.HasValue) return true;

            string value = (model.Value ?? string.Empty).Trim();

            return Extensions.SplitOnWhitespace(value).Count() == 1;
        }

        public string Resolve(OptionModel model)
            => model.HasValue ? model.Value.AsSpan().RemoveLiteralsAndQuotes() : null;
    }
}
