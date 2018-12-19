using System;
using System.Linq.Expressions;

namespace MatthiWare.CommandLine.Abstractions.Command
{
    /// <summary>
    /// Configures commands using a fluent interface
    /// </summary>
    /// <typeparam name="TSource">Command options class</typeparam>
    public interface ICommandBuilder<TOption, TSource> : ICommandConfigurationBuilder, ICommandExecutor<TOption, TSource>
        where TOption : class
        where TSource : class, new()
    {

        /// <summary>
        /// Configures an option in the model
        /// </summary>
        /// <typeparam name="TProperty">Type of the property</typeparam>
        /// <param name="selector">Model property to configure</param>
        /// <returns><see cref="IOptionBuilder"/></returns>
        IOptionBuilder Configure<TProperty>(Expression<Func<TSource, TProperty>> selector);

        /// <summary>
        /// Configures if the command is required
        /// </summary>
        /// <param name="required">True or false</param>
        /// <returns><see cref="ICommandBuilder{TOption, TSource}"/></returns>
        new ICommandBuilder<TOption, TSource> Required(bool required = true);

        /// <summary>
        /// Describes the command, used in the usage output. 
        /// </summary>
        /// <param name="desc">description of the command</param>
        /// <returns><see cref="ICommandBuilder{TOption, TSource}"/></returns>
        new ICommandBuilder<TOption, TSource> Description(string desc);

        /// <summary>
        /// Configures the command name
        /// </summary>
        /// <param name="shortName">Short name</param>
        /// <returns><see cref="ICommandBuilder{TOption, TSource}"/></returns>
        new ICommandBuilder<TOption, TSource> Name(string shortName);

        /// <summary>
        /// Configures the command name
        /// </summary>
        /// <param name="shortName">Short name</param>
        /// <param name="longName">Long name</param>
        /// <returns><see cref="ICommandBuilder{TOption, TSource}"/></returns>
        new ICommandBuilder<TOption, TSource> Name(string shortName, string longName);
    }
}
