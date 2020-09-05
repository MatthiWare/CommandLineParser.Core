using MatthiWare.CommandLine.Abstractions;
using MatthiWare.CommandLine.Abstractions.Command;
using MatthiWare.CommandLine.Abstractions.Parsing;
using MatthiWare.CommandLine.Abstractions.Usage;
using MatthiWare.CommandLine.Core.Command;
using MatthiWare.CommandLine.Core.Usage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace MatthiWare.CommandLine.Core.Parsing.Resolvers
{
    public static class ResolverExtensions
    {
        public static void AddInternalCommandLineParserServices<TOption>(this IServiceCollection services, CommandLineParser<TOption> parser, CommandLineParserOptions options)
            where TOption : class, new()
        {
            services.AddSingleton<ICommandLineCommandContainer>(parser);
            services.AddSingleton<ICommandLineParser<TOption>>(parser);

            services.AddSingleton(options);

            services.AddDefaultResolvers();
            services.AddCLIPrinters();
            services.AddCommandDiscoverer();
        }

        public static void AddDefaultResolvers(this IServiceCollection services)
        {
            services.TryAddScoped<IArgumentResolver<bool>, BoolResolver>();
            services.TryAddScoped<IArgumentResolver<double>, DoubleResolver>();
            services.TryAddScoped<IArgumentResolver<int>, IntResolver>();
            services.TryAddScoped<IArgumentResolver<string>, StringResolver>();

            services.TryAddScoped(typeof(IArgumentResolver<>), typeof(DefaultResolver<>));
        }

        public static void AddCLIPrinters(this IServiceCollection services)
        {
            services.TryAddScoped<IUsageBuilder, UsageBuilder>();
            services.TryAddScoped<IUsagePrinter, UsagePrinter>();
        }

        public static void AddCommandDiscoverer(this IServiceCollection services)
        {
            services.TryAddScoped<ICommandDiscoverer, CommandDiscoverer>();
        }

        public static void AddEnvironmentVariables(this IServiceCollection services)
        {
            services.TryAddScoped<IEnvironmentVariablesService, EnvironmentVariableService>();
        }
    }
}
