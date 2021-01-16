using Microsoft.Extensions.DependencyInjection;
using MatthiWare.CommandLine.Core;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace MatthiWare.CommandLine
{
    public static class DependencyInjectionExtensions
    {
        public static void AddCommandLineParser<TOption>(this IServiceCollection services, CommandLineParserOptions options)
            where TOption : class, new()
        {
            Func<IServiceProvider, CommandLineParser<TOption>> resolver = (IServiceProvider provider) 
                => new CommandLineParser<TOption>(provider.GetService<CommandLineParserOptions>(), provider);
            services.AddInternalCommandLineParserServices(resolver, options);
        }
    }
}
