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
        protected ICommandLineCommandContainer Container { get; }

        /// <inheritdoc/>
        public IUsageBuilder Builder { get; }

        /// <inheritdoc/>
        public UsagePrinter(ICommandLineCommandContainer container, IUsageBuilder builder)
        {
            Container = container;
            Builder = builder;
        }

        /// <inheritdoc/>
        public virtual void PrintErrors(IReadOnlyCollection<Exception> errors)
        {
            var previousColor = Console.ForegroundColor;

            Console.ForegroundColor = ConsoleColor.Red;

            Builder.AddErrors(errors);

            Console.Error.WriteLine(Builder.Build());

            Console.ForegroundColor = previousColor;

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
