using MatthiWare.CommandLine.Abstractions.Command;
using MatthiWare.CommandLine.Abstractions.Usage;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace MatthiWare.CommandLine.Core.Usage
{
    internal class DamerauLevenshteinSuggestionProvider : ISuggestionProvider
    {
        private readonly ILogger<CommandLineParser> logger;
        private readonly ICommandLineCommandContainer container;

        public DamerauLevenshteinSuggestionProvider(ILogger<CommandLineParser> logger, ICommandLineCommandContainer container)
        {
            this.logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
            this.container = container ?? throw new System.ArgumentNullException(nameof(container));
        }

        public IEnumerable<string> GetSuggestions(string input)
        {
            throw new System.NotImplementedException();
        }
    }
}
