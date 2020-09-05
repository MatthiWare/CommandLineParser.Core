using MatthiWare.CommandLine.Core;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace MatthiWare.CommandLine.Tests.Parsing.Resolvers
{
    public abstract class BaseResolverTests
    {
        public IServiceProvider ServiceProvider { get; }

        public BaseResolverTests()
        {
            var services = new ServiceCollection();

            services.AddDefaultResolvers();

            ServiceProvider = services.BuildServiceProvider();
        }
    }
}
