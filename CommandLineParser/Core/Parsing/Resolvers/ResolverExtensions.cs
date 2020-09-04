using MatthiWare.CommandLine.Abstractions.Parsing;
using Microsoft.Extensions.DependencyInjection;

namespace MatthiWare.CommandLine.Core.Parsing.Resolvers
{
    public static class ResolverExtensions
    {
        public static void AddDefaultResolvers(this IServiceCollection services)
        {
            services.AddTransient<IArgumentResolver<bool>, BoolResolver>();
            services.AddTransient<IArgumentResolver<double>, DoubleResolver>();
            services.AddTransient<IArgumentResolver<int>, IntResolver>();
            services.AddTransient<IArgumentResolver<string>, StringResolver>();

            services.AddTransient(typeof(IArgumentResolver<>), typeof(DefaultResolver<>));
        }
    }
}
