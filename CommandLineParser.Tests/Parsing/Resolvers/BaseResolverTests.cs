using MatthiWare.CommandLine.Core;
using Microsoft.Extensions.DependencyInjection;
using System;
using Xunit.Abstractions;

namespace MatthiWare.CommandLine.Tests.Parsing.Resolvers
{
    public abstract class BaseResolverTests : TestBase
    {
        public IServiceProvider ServiceProvider { get; }

        public BaseResolverTests(ITestOutputHelper outputHelper)
            : base(outputHelper)
        {
            Services.AddDefaultResolvers();

            ServiceProvider = Services.BuildServiceProvider();
        }
    }
}
