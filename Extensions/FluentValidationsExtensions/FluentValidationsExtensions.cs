using MatthiWare.CommandLine.Abstractions;
using MatthiWare.CommandLine.Extensions.FluentValidations.Core;
using System;

namespace MatthiWare.CommandLine.Extensions.FluentValidations
{
    /// <summary>
    /// FluentValidations Extensions for CommandLineParser
    /// </summary>
    public static class FluentValidationsExtensions
    {
        /// <summary>
        /// Extensions to configure FluentValidations for the Parser
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parser"></param>
        /// <param name="configAction">Configuration action</param>
        public static void UseFluentValidations<T>(this ICommandLineParser<T> parser, Action<FluentValidationConfiguration> configAction)
            where T : class, new()
        {
            var config = new FluentValidationConfiguration(parser.Validators, parser.Services);

            configAction(config);
        }
    }
}
