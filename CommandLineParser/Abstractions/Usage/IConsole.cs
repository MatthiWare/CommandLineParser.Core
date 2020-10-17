using System;

namespace MatthiWare.CommandLine.Abstractions.Usage
{
    /// <summary>
    /// <inheritdoc cref="Console"/>
    /// </summary>
    public interface IConsole
    {
        /// <summary>
        /// <inheritdoc cref="Console.WriteLine()"/>
        /// </summary>
        void WriteLine();

        /// <summary>
        /// <inheritdoc cref="Console.WriteLine(string)"/>
        /// </summary>
        /// <param name="text">Input text</param>
        void WriteLine(string text);

        /// <summary>
        /// <inheritdoc cref="Console.Error"/>
        /// </summary>
        /// <param name="text">Input text</param>
        void ErrorWriteLine(string text);

        /// <summary>
        /// <inheritdoc cref="Console.ForegroundColor"/>
        /// </summary>
        ConsoleColor ForegroundColor { get; set; }

        /// <summary>
        /// <inheritdoc cref="Console.ResetColor"/>
        /// </summary>
        void ResetColor();
    }
}
