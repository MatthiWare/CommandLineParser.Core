using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using MatthiWare.CommandLine.Abstractions;
using MatthiWare.CommandLine.Abstractions.Command;
using MatthiWare.CommandLine.Abstractions.Models;
using MatthiWare.CommandLine.Abstractions.Parsing;
using MatthiWare.CommandLine.Abstractions.Parsing.Command;
using MatthiWare.CommandLine.Core.Exceptions;

namespace MatthiWare.CommandLine.Core.Command
{
    internal class CommandLineCommand<TSource> :
        CommandLineCommandBase,
        ICommandBuilder<TSource> where TSource : class, new()
    {
        private readonly TSource source;
        private readonly IResolverFactory resolverFactory;
        private Action<TSource> execution;

        public CommandLineCommand(IResolverFactory resolverFactory)
        {
            this.resolverFactory = resolverFactory;
            source = new TSource();
        }

        public IOptionBuilder Configure<TProperty>(Expression<Func<TSource, TProperty>> selector)
        {
            var option = new CommandLineOption(source, selector, resolverFactory.CreateResolver<TProperty>());

            m_options.Add(option);

            return option;
        }

        public override void Execute() => execution(source);

        public override ICommandParserResult Parse(IArgumentManager argumentManager)
        {
            var result = new CommandParserResult(this);
            var errors = new List<Exception>();

            foreach (var option in m_options)
            {
                if (!argumentManager.TryGetValue(option, out ArgumentModel model) && option.IsRequired)
                {
                    errors.Add(new OptionNotFoundException(option));

                    continue;
                }
                else if (!model.HasValue && option.HasDefault)
                {
                    option.UseDefault();

                    continue;
                }
                else if (!option.CanParse(model))
                {
                    errors.Add(new OptionParseException(option, model));

                    continue;
                }

                option.Parse(model);
            }

            result.MergeResult(errors);

            return result;
        }

        public ICommandBuilder<TSource> OnExecuting(Action<TSource> action)
        {
            execution = action;

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

        ICommandBuilder<TSource> ICommandBuilder<TSource>.Name(string shortName)
        {
            ShortName = shortName;

            return this;
        }

        ICommandBuilder<TSource> ICommandBuilder<TSource>.Name(string shortName, string longName)
        {
            ShortName = shortName;
            LongName = longName;

            return this;
        }
    }
}
