using MatthiWare.CommandLine.Abstractions;
using MatthiWare.CommandLine.Abstractions.Command;
using MatthiWare.CommandLine.Abstractions.Usage;
using System;
using System.Collections.Generic;

namespace MatthiWare.CommandLine.Core.Usage
{
    /// <inheritdoc/>
    public class UsagePrinter : IUsagePrinter
    {
        private readonly IEnvironmentVariablesService environmentVariablesService;

        protected ICommandLineCommandContainer Container { get; }

        /// <inheritdoc/>
        public IUsageBuilder Builder { get; }

        /// <summary>
        /// Creates a new CLI output usage printer
        /// </summary>
        /// <param name="container"></param>
        /// <param name="builder"></param>
        public UsagePrinter(ICommandLineCommandContainer container, IUsageBuilder builder)
            : this(container, builder, new EnvironmentVariableService())
        { }

        /// <summary>
        /// Creates a new CLI output usage printer
        /// </summary>
        /// <param name="container"></param>
        /// <param name="builder"></param>
        /// <param name="environmentVariablesService"></param>
        public UsagePrinter(ICommandLineCommandContainer container, IUsageBuilder builder, IEnvironmentVariablesService environmentVariablesService)
        {
            Container = container ?? throw new ArgumentNullException(nameof(container));
            Builder = builder ?? throw new ArgumentNullException(nameof(builder));
            this.environmentVariablesService = environmentVariablesService ?? throw new ArgumentNullException(nameof(environmentVariablesService));
        }

        /// <inheritdoc/>
        public virtual void PrintErrors(IReadOnlyCollection<Exception> errors)
        {
            bool canOutputColor = !this.environmentVariablesService.NoColorRequested;

            var previousColor = Console.ForegroundColor;

            if (canOutputColor)
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }

            Builder.AddErrors(errors);

            Console.Error.WriteLine(Builder.Build());

            if (canOutputColor)
            { 
                Console.ForegroundColor = previousColor;
            }

            Console.WriteLine();
        }

        /// <inheritdoc/>
        public virtual void PrintCommandUsage(ICommandLineCommand command)
        {
            Builder.AddCommand(command.Name, command);
            Console.WriteLine(Builder.Build());
        }

        /// <inheritdoc/>
        public virtual void PrintOptionUsage(ICommandLineOption option)
        {
            Builder.AddOption(option);
            Console.WriteLine(Builder.Build());
        }

        /// <inheritdoc/>
        public virtual void PrintUsage()
        {
            Builder.AddCommand(string.Empty, Container);
            Console.WriteLine(Builder.Build());
        }

        /// <inheritdoc/>
        public virtual void PrintUsage(IArgument argument)
        {
            switch (argument)
            {
                case ICommandLineCommand cmd:
                    PrintCommandUsage(cmd);
                    break;
                case ICommandLineOption opt:
                    PrintOptionUsage(opt);
                    break;
                default:
                    PrintUsage();
                    break;
            }
        }

        /// <inheritdoc/>
        public virtual void PrintUsage(ICommandLineCommand command)
            => PrintCommandUsage(command);

        /// <inheritdoc/>
        public virtual void PrintUsage(ICommandLineOption option)
            => PrintOptionUsage(option);
    }
}
