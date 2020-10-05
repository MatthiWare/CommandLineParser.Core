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

        public override bool CanResolve(ArgumentModel model) => CanResolve(model.Values.FirstOrDefault());

        public override bool CanResolve(string value) => TryParse(value, out _);

        public override bool Resolve(ArgumentModel model) => Resolve(model.Values.FirstOrDefault());

        public override bool Resolve(string value)
        {
            TryParse(value, out bool result);

            return result;
        }

        private bool TryParse(string value, out bool result)
        {
            if (recognisedFalseArgs.Contains(value, StringComparer.InvariantCultureIgnoreCase))
            {
                logger.LogDebug("BoolResolver does not recognize {input} as false", value);
                result = false;
                return true;
            }

            if (recognisedTrueArgs.Contains(value, StringComparer.OrdinalIgnoreCase))
            {
                logger.LogDebug("BoolResolver does not recognize {input} as true", value);
                result = true;
                return true;
            }

            return bool.TryParse(value, out result);
        }
    }
}
