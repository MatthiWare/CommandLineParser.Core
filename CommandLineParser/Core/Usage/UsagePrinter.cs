using MatthiWare.CommandLine.Abstractions;
using MatthiWare.CommandLine.Abstractions.Command;
using MatthiWare.CommandLine.Abstractions.Parsing;
using MatthiWare.CommandLine.Abstractions.Usage;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MatthiWare.CommandLine.Core.Usage
{
    /// <inheritdoc/>
    public class UsagePrinter : IUsagePrinter
    {
        private readonly IEnvironmentVariablesService environmentVariablesService;
        private readonly ISuggestionProvider suggestionProvider;

        protected ICommandLineCommandContainer Container { get; }

        /// <inheritdoc/>
        public IUsageBuilder Builder { get; }

        internal ConsoleColor m_currentConsoleColor = ConsoleColor.Gray;

        /// <summary>
        /// Creates a new CLI output usage printer
        /// </summary>
        /// <param name="container"></param>
        /// <param name="builder"></param>
        /// <param name="environmentVariablesService"></param>
        public UsagePrinter(ICommandLineCommandContainer container, IUsageBuilder builder, IEnvironmentVariablesService environmentVariablesService, ISuggestionProvider suggestionProvider)
        {
            Container = container ?? throw new ArgumentNullException(nameof(container));
            Builder = builder ?? throw new ArgumentNullException(nameof(builder));
            this.environmentVariablesService = environmentVariablesService ?? throw new ArgumentNullException(nameof(environmentVariablesService));
            this.suggestionProvider = suggestionProvider ?? throw new ArgumentNullException(nameof(suggestionProvider));
        }

        /// <inheritdoc/>
        public virtual void PrintErrors(IReadOnlyCollection<Exception> errors)
        {
            bool canOutputColor = !this.environmentVariablesService.NoColorRequested;

            if (canOutputColor)
            {
                m_currentConsoleColor = ConsoleColor.Red;
                Console.ForegroundColor = ConsoleColor.Red;
            }

            Builder.AddErrors(errors);

            Console.Error.WriteLine(Builder.Build());

            if (canOutputColor)
            {
                m_currentConsoleColor = ConsoleColor.Gray;
                Console.ResetColor();
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

        public void PrintSuggestion(UnusedArgumentModel model)
        {
            var suggestions = suggestionProvider.GetSuggestions(model.Key, model.Argument as ICommandLineCommandContainer);

            if (!suggestions.Any())
            {
                return;
            }

            Builder.AddSuggestion(model.Key);

            foreach (var suggestion in suggestions)
            {
                Builder.AddSuggestion(suggestion);
            }
        }
    }
}
