using System;
using System.Linq;

using MatthiWare.CommandLine.Abstractions.Models;
using MatthiWare.CommandLine.Abstractions.Parsing;
using Microsoft.Extensions.Logging;

namespace MatthiWare.CommandLine.Core.Parsing.Resolvers
{
    internal class BoolResolver : BaseArgumentResolver<bool>
    {
        private static readonly string[] recognisedFalseArgs = new[] { "off", "0", "false", "no" };
        private static readonly string[] recognisedTrueArgs = new[] { "on", "1", "true", "yes", string.Empty, null };
        private readonly ILogger logger;

        public BoolResolver(ILogger<CommandLineParser> logger)
        {
            this.logger = logger;
        }

        public override bool CanResolve(ArgumentModel model) => TryParse(model, out _);

        public override bool Resolve(ArgumentModel model)
        {
            TryParse(model, out bool result);

            return result;
        }

        private bool TryParse(ArgumentModel model, out bool result)
        {
            if (recognisedFalseArgs.Contains(model.Value, StringComparer.InvariantCultureIgnoreCase))
            {
                logger.LogDebug("BoolResolver does not recognize {input} as false", model.Value);
                result = false;
                return true;
            }

            if (recognisedTrueArgs.Contains(model.Value, StringComparer.OrdinalIgnoreCase))
            {
                logger.LogDebug("BoolResolver does not recognize {input} as true", model.Value);
                result = true;
                return true;
            }

            return bool.TryParse(model.Value, out result);
        }
    }
}
