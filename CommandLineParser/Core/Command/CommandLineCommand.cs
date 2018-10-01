using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using MatthiWare.CommandLine.Abstractions;
using MatthiWare.CommandLine.Abstractions.Command;
using MatthiWare.CommandLine.Abstractions.Parsing;

namespace MatthiWare.CommandLine.Core.Command
{
    internal class CommandLineCommand<TSource> : ICommandLineCommand, ICommandBuilder<TSource> where TSource : class, new()
    {
        private readonly IResolverFactory resolverFactory;
        private Action<TSource> execution;
        private readonly List<ICommandLineArgumentOption> options;
        private TSource source;

        public CommandLineCommand(IResolverFactory resolverFactory, List<ICommandLineArgumentOption> options)
        {
            this.resolverFactory = resolverFactory;
            this.options = options;
            this.source = new TSource();
        }

        public string ShortName { get; set; }
        public string LongName { get; set; }
        public string HelpText { get; set; }
        public bool IsRequired { get; set; }

        public bool HasShortName => ShortName != null;

        public bool HasLongName => LongName != null;

        public IOptionBuilder<TProperty> Configure<TProperty>(Expression<Func<TSource, TProperty>> selector)
        {
            var option = new CommandLineArgumentOption<TSource, TProperty>(source, selector, resolverFactory.CreateResolver<TProperty>());

            options.Add(option);

            return option;

        }

        public void Execute() => execution(source);

        public ICommandBuilder<TSource> OnExecuting(Action<TSource> action)
        {
            this.execution = action;

            return this;
        }

        public ICommandBuilder<TSource> Required(bool required = true)
        {
            IsRequired = required;

            return this;
        }

        ICommandBuilder<TSource> ICommandBuilder<TSource>.HelpText(string help)
        {
            HelpText = help;

            return this;
        }

        ICommandBuilder<TSource> ICommandBuilder<TSource>.LongName(string longName)
        {
            LongName = longName;

            return this;
        }

        ICommandBuilder<TSource> ICommandBuilder<TSource>.ShortName(string shortName)
        {
            ShortName = shortName;

            return this;
        }
    }
}
