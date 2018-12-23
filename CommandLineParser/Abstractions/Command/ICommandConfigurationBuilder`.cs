using System;
using System.Linq.Expressions;

namespace MatthiWare.CommandLine.Abstractions.Command
{
    public interface ICommandConfigurationBuilder<TSource>
        : ICommandConfigurationBuilder
        where TSource : class
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
        /// <returns><see cref="ICommandConfigurationBuilder"/></returns>
        new ICommandConfigurationBuilder<TSource> Required(bool required = true);

        /// <summary>
        /// Configures the description text for the command
        /// </summary>
        /// <param name="required">True or false</param>
        /// <returns><see cref="ICommandConfigurationBuilder"/></returns>
        new ICommandConfigurationBuilder<TSource> Description(string description);

        /// <summary>
        /// Configures the command name
        /// </summary>
        /// <param name="name">Short name</param>
        /// <returns><see cref="ICommandConfigurationBuilder"/></returns>
        new ICommandConfigurationBuilder<TSource> Name(string name);
    }
}