using System;
using System.Collections.Generic;
using MatthiWare.CommandLine.Abstractions.Command;

namespace MatthiWare.CommandLine.Abstractions.Usage
{
    /// <summary>
    /// Output builder
    /// </summary>
    public interface IUsageBuilder
    {
        /// <summary>
        /// Generates the output 
        /// </summary>
        /// <returns>Output string</returns>
        string Build();

        /// <summary>
        /// Add usage
        /// </summary>
        /// <param name="name">Name of the applpication</param>
        /// <param name="hasOptions">Indicates if the output contains options</param>
        /// <param name="hasCommands">Indicates if the output contains commands</param>
        void AddUsage(string name, bool hasOptions, bool hasCommands);

        /// <summary>
        /// Add all options
        /// </summary>
        /// <param name="options"></param>
        void AddOptions(IEnumerable<ICommandLineOption> options);

        /// <summary>
        /// Add a specific option
        /// </summary>
        /// <param name="option"></param>
        void AddOption(ICommandLineOption option);

        /// <summary>
        /// Adds all command descriptions
        /// </summary>
        /// <param name="commands"></param>
        void AddCommandDescriptions(IEnumerable<ICommandLineCommand> commands);

        /// <summary>
        /// Adds a specific command description
        /// </summary>
        /// <param name="command"></param>
        void AddCommandDescription(ICommandLineCommand command);

        /// <summary>
        /// Adds a command to the output builder
        /// </summary>
        /// <param name="name"></param>
        /// <param name="command"></param>
        void AddCommand(string name, ICommandLineCommand command);

        /// <summary>
        /// Adds a command to the output builder
        /// </summary>
        /// <param name="name"></param>
        /// <param name="container"></param>
        void AddCommand(string name, ICommandLineCommandContainer container);

        /// <summary>
        /// Adds the errors to the output builder
        /// </summary>
        /// <param name="errors"></param>
        void AddErrors(IReadOnlyCollection<Exception> errors);
    }
}
