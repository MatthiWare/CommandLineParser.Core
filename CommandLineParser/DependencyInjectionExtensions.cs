using Microsoft.Extensions.DependencyInjection;
using MatthiWare.CommandLine.Core;
using System;
using MatthiWare.CommandLine.Abstractions;

namespace MatthiWare.CommandLine
{
    /// <summary>
    /// Extension methods to allow DI services to be registered.
    /// </summary>
    public static class DependencyInjectionExtensions
    {
        /// <summary>
        /// Adds <see cref="ICommandLineParser{TOption}"/> to the services
        /// This won't overwrite existing services.
        /// </summary>
        /// <typeparam name="TOption">Base option type</typeparam>
        /// <param name="services">Current service collection</param>
        /// <param name="options">Current options reference</param>
        public static void AddCommandLineParser<TOption>(this IServiceCollection services, CommandLineParserOptions options = null)
            where TOption : class, new()
        {
            Func<IServiceProvider, CommandLineParser<TOption>> factory = (IServiceProvider provider)
                => new CommandLineParser<TOption>(provider.GetService<CommandLineParserOptions>(), provider);

            services
                .AddInternalCommandLineParserServices2(options)
                .AddCommandLineParserFactoryGeneric(factory);
        }

        /// <summary>
        /// Adds <see cref="CommandLineParser"/> to the services
        /// This won't overwrite existing services.
        /// </summary>
        /// <param name="services">Current service collection</param>
        /// <param name="options">Current options reference</param>
        public static void AddCommandLineParser(this IServiceCollection services, CommandLineParserOptions options = null)
        {
            Func<IServiceProvider, CommandLineParser> factory = (IServiceProvider provider)
                => new CommandLineParser(provider.GetService<CommandLineParserOptions>(), provider);

            services
                .AddInternalCommandLineParserServices2(options)
                .AddCommandLineParserFactory(factory);
        }
    }
}
