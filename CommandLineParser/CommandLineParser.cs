using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using MatthiWare.CommandLine.Abstractions;
using MatthiWare.CommandLine.Abstractions.Command;
using MatthiWare.CommandLine.Abstractions.Models;
using MatthiWare.CommandLine.Abstractions.Parsing;
using MatthiWare.CommandLine.Core;
using MatthiWare.CommandLine.Core.Command;
using MatthiWare.CommandLine.Core.Parsing;

[assembly: InternalsVisibleTo("CommandLineParser.Tests")]

namespace MatthiWare.CommandLine
{
    public sealed class CommandLineParser<TSource> : ICommandLineParser<TSource> where TSource : class, new()
    {
        private readonly TSource m_option;
        private readonly List<CommandLineArgumentOptionBase> m_options;
        private readonly List<CommandLineCommandBase> m_commands;

        public IReadOnlyList<ICommandLineArgumentOption> Options => m_options.AsReadOnly();

        public IResolverFactory ResolverFactory { get; }

        public IReadOnlyList<ICommandLineCommand> Commands => m_commands.AsReadOnly();

        public CommandLineParser()
        {
            m_option = new TSource();

            m_options = new List<CommandLineArgumentOptionBase>();
            m_commands = new List<CommandLineCommandBase>();

            ResolverFactory = new ResolverFactory();
        }

        public IOptionBuilder<TProperty> Configure<TProperty>(Expression<Func<TSource, TProperty>> selector)
            => ConfigureInternal(selector);

        private IOptionBuilder<TProperty> ConfigureInternal<TProperty>(Expression<Func<TSource, TProperty>> selector)
        {
            var option = new CommandLineArgumentOption<TSource, TProperty>(m_option, selector, ResolverFactory.CreateResolver<TProperty>());

            m_options.Add(option);

            return option;
        }

        public IParserResult<TSource> Parse(string[] args)
        {
            var lstArgs = new List<string>(args);
            var errors = new List<Exception>();

            var result = new ParseResult<TSource>();

            foreach (var option in m_options)
            {
                int idx = lstArgs.FindIndex(arg =>
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

                if (!option.CanParse(argModel))
                {
                    errors.Add(new ArgumentException($"Cannot parse option '{argModel.Key}:{argModel.Value ?? "NULL"}'."));

                    continue;
                }

                option.Parse(argModel);
            }

            result.MergeResult(m_option);

            foreach (var cmd in m_commands)
            {
                int idx = lstArgs.FindIndex(arg =>
                    (cmd.HasShortName && string.Equals(cmd.ShortName, arg, StringComparison.InvariantCultureIgnoreCase)) ||
                    (cmd.HasLongName && string.Equals(cmd.LongName, arg, StringComparison.InvariantCultureIgnoreCase)));

                if (idx < 0 || idx > lstArgs.Count)
                {
                    if (cmd.IsRequired)
                        errors.Add(new KeyNotFoundException($"Required command '{cmd.HasShortName}' or '{cmd.LongName}' not found!"));

                    continue;
                }

                result.MergeResult(cmd.Parse(lstArgs));
            }

            return result;
        }

        public ICommandBuilder<TCommandOption> AddCommand<TCommandOption>() where TCommandOption : class, new()
        {
            var command = new CommandLineCommand<TCommandOption>(ResolverFactory);

            m_commands.Add(command);

            return command;
        }
    }
}
