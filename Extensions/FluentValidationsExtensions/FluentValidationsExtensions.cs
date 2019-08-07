using FluentValidation;
using MatthiWare.CommandLine.Abstractions;
using MatthiWare.CommandLine.Extensions.FluentValidations.Core;
using System;

namespace MatthiWare.CommandLine.Extensions.FluentValidations
{
    public static class FluentValidationsExtensions
    {
        public static void UseFluentValidations<T>(this CommandLineParser<T> parser, Action<FluentValidationConfiguration> configAction)
            where T : class, new()
        {
            var config = new FluentValidationConfiguration(parser.Validators);

            configAction(config);
        }
    }
}
