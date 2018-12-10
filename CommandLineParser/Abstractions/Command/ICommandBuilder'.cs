using System;
using System.Linq.Expressions;

namespace MatthiWare.CommandLine.Abstractions.Command
{
    /// <summary>
    /// Configures commands using a fluent interface
    /// </summary>
    /// <typeparam name="TSource">Command options class</typeparam>
    public interface ICommandBuilder<TOption, TSource>
        where TOption : class
        where TSource : class, new()
    {
        /// <summary>
        /// Configures if the command is required
        /// </summary>
        /// <param name="required">True or false</param>
        /// <returns><see cref="ICommandBuilder{TOption, TSource}"/></returns>
        ICommandBuilder<TOption, TSource> Required(bool required = true);

        /// <summary>
        /// Configures the help text for the command
        /// </summary>
        /// <param name="required">True or false</param>
        /// <returns><see cref="ICommandBuilder{TOption, TSource}"/></returns>
        ICommandBuilder<TOption, TSource> HelpText(string help);

        /// <summary>
        /// Configures the command name
        /// </summary>
        /// <param name="shortName">Short name</param>
        /// <returns><see cref="ICommandBuilder{TOption, TSource}"/></returns>
        ICommandBuilder<TOption, TSource> Name(string shortName);

        /// <summary>
        /// Configures the command name
        /// </summary>
        /// <param name="shortName">Short name</param>
        /// <param name="longName">Long name</param>
        /// <returns><see cref="ICommandBuilder{TOption, TSource}"/></returns>
        ICommandBuilder<TOption, TSource> Name(string shortName, string longName);

        /// <summary>
        /// Configures the execution of the command
        /// </summary>
        /// <param name="required">True or false</param>
        /// <returns><see cref="ICommandBuilder{TOption, TSource}"/></returns>
        ICommandBuilder<TOption, TSource> OnExecuting(Action<TOption> action);

        /// <summary>
        /// Configures the execution of the command
        /// </summary>
        /// <param name="required">True or false</param>
        /// <returns><see cref="ICommandBuilder{TOption, TSource}"/></returns>
        ICommandBuilder<TOption, TSource> OnExecuting(Action<TOption, TSource> action);

        /// <summary>
        /// Configures if the command options
        /// </summary>
        /// <param name="selector">Property to configure</param>
        /// <returns><see cref="IOptionBuilder"/></returns>
        IOptionBuilder Configure<TProperty>(Expression<Func<TSource, TProperty>> selector);
    }
}
