using MatthiWare.CommandLine.Abstractions.Usage;
using System;

namespace MatthiWare.CommandLine.Core.Usage
{
    /// <inheritdoc/>
    public class SystemConsole : IConsole
    {
        /// <inheritdoc/>
        public ConsoleColor ForegroundColor 
        { 
            get => Console.ForegroundColor; 
            set => Console.ForegroundColor = value;
        }

        /// <inheritdoc/>
        public void ErrorWriteLine(string text) => Console.Error.WriteLine(text);

        /// <inheritdoc/>
        public void ResetColor() => Console.ResetColor();

        /// <inheritdoc/>
        public void WriteLine(string text) => Console.WriteLine(text);

        /// <inheritdoc/>
        public void WriteLine() => Console.WriteLine();
    }
}
