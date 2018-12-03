using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using MatthiWare.CommandLine.Abstractions.Command;
using MatthiWare.CommandLine.Abstractions.Parsing;

namespace MatthiWare.CommandLine.Abstractions
{
    public interface ICommandLineParser<TSource>
    {
        #region Properties

        /// <summary>
        /// Read-only list of available sub-commands
        /// <see cref="ICommandLineParser{TSource}.AddCommand{TCommandOption}"/> to configure or add an command
        /// </summary>
        IReadOnlyList<ICommandLineCommand> Commands { get; }

        /// <summary>
        /// Read-only list of available options for this command
        /// <see cref="ICommandLineParser{TSource}.Configure{TProperty}(Expression{Func{TSource, TProperty}})"/> to configure or add an option
        /// </summary>
        IReadOnlyList<ICommandLineOption> Options { get; }

        /// <summary>
        /// Factory to resolve the argument type
        /// <see cref="ICommandLineArgumentResolver"/> for more info.
        /// </summary>
        IResolverFactory ResolverFactory { get; }

        #endregion

        #region Parsing

        /// <summary>
        /// Parses the current command and returns the result
        /// </summary>
        /// <param name="args">command arguments</param>
        /// <returns>The <see cref="IParserResult{TResult}"/> result</returns>
        IParserResult<TSource> Parse(string[] args);

        #endregion

        #region Configuration

        /// <summary>
        /// Configures a new or existing option
        /// </summary>
        /// <typeparam name="TProperty">The property type</typeparam>
        /// <param name="selector">Property selector</param>
        /// <returns><see cref="IOptionBuilder"/></returns>
        IOptionBuilder Configure<TProperty>(Expression<Func<TSource, TProperty>> selector);

        /// <summary>
        /// Adds a new command  and allowes to configure it. 
        /// </summary>
        /// <typeparam name="TCommandOption">Command options model</typeparam>
        /// <returns>A command builder, see <see cref="ICommandBuilder{Tsource}"/> for more info.</returns>
        ICommandBuilder<TCommandOption> AddCommand<TCommandOption>() where TCommandOption : class, new();

        #endregion
    }
}
