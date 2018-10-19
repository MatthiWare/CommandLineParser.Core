using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MatthiWare.CommandLine.Abstractions;
using MatthiWare.CommandLine.Abstractions.Command;
using MatthiWare.CommandLine.Abstractions.Models;
using MatthiWare.CommandLine.Abstractions.Parsing;
using MatthiWare.CommandLine.Abstractions.Parsing.Command;

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

        public IOptionBuilder<TProperty> Configure<TProperty>(Expression<Func<TSource, TProperty>> selector)
        {
            var option = new CommandLineOption<TSource, TProperty>(source, selector, resolverFactory.CreateResolver<TProperty>());

            options.Add(option);

            return option;
        }

        public override void Execute() => execution(source);

        public override ICommandParserResult Parse(List<string> lstArgs, int startIndex)
        {
            var result = new CommandParserResult(this);
            var errors = new List<Exception>();

            foreach (var option in Options)
            {
                int idx = lstArgs.FindIndex(startIndex, arg =>
                    (option.HasShortName && string.Equals(option.ShortName, arg, StringComparison.InvariantCultureIgnoreCase)) ||
                    (option.HasLongName && string.Equals(option.LongName, arg, StringComparison.InvariantCultureIgnoreCase)));

                if (idx < 0 || idx > lstArgs.Count)
                {
                    if (option.IsRequired)
                        errors.Add(new KeyNotFoundException($"Required argument '{option.HasShortName}' or '{option.LongName}' not found!"));

                    continue;
                }

                var key = lstArgs.GetAndRemove(idx);
                var value = lstArgs.GetAndRemove(idx);

                var argModel = new ArgumentModel(key, value);

                var parser = option as IParser;

                if (!parser.CanParse(argModel))
                {
                    errors.Add(new ArgumentException($"Cannot parse option '{argModel.Key}:{argModel.Value ?? "NULL"}'."));

                    continue;
                }

                parser.Parse(argModel);
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
