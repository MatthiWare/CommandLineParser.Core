using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using MatthiWare.CommandLine.Abstractions.Command;
using MatthiWare.CommandLine.Abstractions.Parsing;
using MatthiWare.CommandLine.Abstractions.Usage;

namespace MatthiWare.CommandLine.Abstractions
{
    public interface ICommandLineParser<TOption>
        where TOption : class, new()
    {
        #region Properties

        /// <summary>
        /// Tool to print usage info.
        /// </summary>
        IUsagePrinter Printer { get; set; }

        /// <summary>
        /// Factory to resolve the argument type
        /// <see cref="ICommandLineArgumentResolver"/> for more info.
        /// </summary>
        IArgumentResolverFactory ArgumentResolverFactory { get; }

        /// <summary>
        /// Resolver that is used to instantiate types by an given container
        /// </summary>
        IContainerResolver ContainerResolver { get; }

        #endregion

        #region Parsing

        /// <summary>
        /// Parses the current command and returns the result
        /// </summary>
        /// <param name="args">command arguments</param>
        /// <returns>The <see cref="IParserResult{TOption}"/> result</returns>
        IParserResult<TOption> Parse(string[] args);

        #endregion

        #region Configuration

        /// <summary>
        /// Configures a new or existing option
        /// </summary>
        /// <typeparam name="TProperty">The property type</typeparam>
        /// <param name="selector">Property selector</param>
        /// <returns><see cref="IOptionBuilder"/></returns>
        IOptionBuilder Configure<TProperty>(Expression<Func<TOption, TProperty>> selector);

        /// <summary>
        /// Adds a new command  and allowes to configure it. 
        /// </summary>
        /// <typeparam name="TCommandOption">Command options model</typeparam>
        /// <returns>A command builder, <see cref="ICommandBuilder{TOption, TSource}"/> for more info.</returns>
        ICommandBuilder<TOption, TCommandOption> AddCommand<TCommandOption>()
            where TCommandOption : class, new();

        /// <summary>
        /// Adds a new command and allowes to configure it. 
        /// </summary>
        /// <returns>A command builder, see <see cref="ICommandBuilder{TOption}"/> for more info.</returns>
        ICommandBuilder<TOption> AddCommand();

        /// <summary>
        /// Registers a new command
        /// </summary>
        /// <typeparam name="TCommand">The command</typeparam>
        void RegisterCommand<TCommand>()
            where TCommand : Command.Command;

        /// <summary>
        /// Registers a new command
        /// </summary>
        /// <typeparam name="TCommand">The command</typeparam>
        /// <typeparam name="TCommandOption">Command options model</typeparam>
        void RegisterCommand<TCommand, TCommandOption>()
            where TCommand : Command<TOption, TCommandOption>
            where TCommandOption : class, new();

        #endregion
    }
}
