using System;
using System.Collections.Generic;

namespace MatthiWare.CommandLine.Abstractions.Command
{
    /// <summary>
    /// Container that holds options and subcommands.
    /// </summary>
    public interface ICommandLineCommandContainer
    {
        /// <summary>
        /// Read-only list of available sub-commands
        /// <see cref="ICommandLineParser{TOption}.AddCommand{TCommandOption}"/> to configure or add an command
        /// </summary>
        IReadOnlyList<ICommandLineCommand> Commands { get; }

        /// <summary>
        /// Read-only list of available options for this command
        /// <see cref="ICommandLineParser{TOption}.Configure{TProperty}(Expression{Func{TOption, TProperty}})"/> to configure or add an option
        /// </summary>
        IReadOnlyList<ICommandLineOption> Options { get; }
    }
}
