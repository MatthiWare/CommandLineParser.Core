using MatthiWare.CommandLine.Abstractions.Usage;
using System;

namespace MatthiWare.CommandLine.Core.Usage
{
    /// <inheritdoc/>
    public class EnvironmentVariableService : IEnvironmentVariablesService
    {
        private const string NoColorId = "NO_COLOR";

        /// <inheritdoc/>
        public bool NoColorRequested => Environment.GetEnvironmentVariable(NoColorId) != null;
    }
}
