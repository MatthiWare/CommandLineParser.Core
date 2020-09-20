using System;
using System.Linq.Expressions;

namespace MatthiWare.CommandLine.Abstractions
{
    /// <summary>
    /// Allows options to be configured
    /// </summary>
    /// <typeparam name="TSource">Source type that contains the option</typeparam>
    public interface IOptionConfigurator<TSource>
    {
        /// <summary>
        /// Configures if the command options
        /// </summary>
        /// <param name="selector">Property to configure</param>
        /// <returns><see cref="IOptionBuilder"/></returns>
        IOptionBuilder<TProperty> Configure<TProperty>(Expression<Func<TSource, TProperty>> selector);
    }
}
