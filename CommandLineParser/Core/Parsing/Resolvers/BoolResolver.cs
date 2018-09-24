using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MatthiWare.CommandLine.Abstractions.Models;
using MatthiWare.CommandLine.Abstractions.Parsing;

namespace MatthiWare.CommandLine.Core.Parsing.Resolvers
{
    internal class BoolResolver : ICommandLineArgumentResolver<bool>
    {
        private static readonly string[] recognisedFalseArgs = new[] { "off", "0", "false", "no" };
        private static readonly string[] recognisedTrueArgs = new[] { "on", "1", "true", "yes" };

        public bool CanResolve(ArgumentModel model) => TryParse(model, out _);

        public bool Resolve(ArgumentModel model)
        {
            TryParse(model, out bool result);

            return result;
        }

        private bool TryParse(ArgumentModel model, out bool result)
        {
            if (!model.HasValue)
            {
                result = false;
                return false;
            }

            if (recognisedFalseArgs.Contains(model.Value, StringComparer.InvariantCultureIgnoreCase))
            {
                result = false;
                return true;
            }

            if (recognisedTrueArgs.Contains(model.Value, StringComparer.OrdinalIgnoreCase))
            {
                result = true;
                return true;
            }

            return bool.TryParse(model.Value, out result);
        }
    }
}
