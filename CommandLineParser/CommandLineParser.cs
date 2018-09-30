using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using MatthiWare.CommandLine.Abstractions;
using MatthiWare.CommandLine.Abstractions.Models;
using MatthiWare.CommandLine.Abstractions.Parsing;
using MatthiWare.CommandLine.Core;
using MatthiWare.CommandLine.Core.Parsing;
using MatthiWare.CommandLine.Core.Parsing.Resolvers;

[assembly: InternalsVisibleTo("CommandLineParser.Tests")]

namespace MatthiWare.CommandLine
{
    public sealed class CommandLineParser<TSource> : ICommandLineParser<TSource> where TSource : class, new()
    {
        private readonly TSource m_option;
        private readonly List<ICommandLineArgumentOption> m_options;

        public IReadOnlyList<ICommandLineArgumentOption> Options => m_options.AsReadOnly();

        public IResolverFactory ResolverFactory { get; }

        public CommandLineParser(Func<TSource> creator)
            : this(creator())
        { }

        public CommandLineParser()
            : this(new TSource())
        { }

        public CommandLineParser(TSource obj)
        {
            m_option = obj ?? throw new ArgumentNullException(nameof(obj));
            m_options = new List<ICommandLineArgumentOption>();
            ResolverFactory = new ResolverFactory();
        }

        public IOptionBuilder<TProperty> Configure<TProperty>(Expression<Func<TSource, TProperty>> selector)
        {
            return ConfigureInternal(selector);
        }

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

            string dash = "-";
            string doubleDash = "--";

            foreach (ICommandLineArgumentOption option in m_options)
            {
                int idx = lstArgs.FindIndex(arg =>
                    (option.HasShortName && string.Equals(option.ShortName, arg, StringComparison.InvariantCultureIgnoreCase)) ||
                    (option.HasLongName && string.Equals(option.LongName, arg, StringComparison.InvariantCultureIgnoreCase)));

                if ((idx < 0 || idx > lstArgs.Count) && option.IsRequired)
                {
                    errors.Add(new KeyNotFoundException($"Required argument '{option.HasShortName}' or '{option.LongName}' not found"));
                    continue;
                }

                var key = lstArgs[idx];
                var value = ++idx < lstArgs.Count ? lstArgs[idx] : null;

                var argModel = new ArgumentModel(key, value);

                var parser = option as IParser;

                if (!parser.CanParse(argModel))
                {
                    errors.Add(new ArgumentException($"Cannot parse option '{argModel.Key}:{argModel.Value ?? "NULL"}'"));

                    continue;
                }

                parser.Parse(argModel);
            }

            if (errors.Any())
                return ParseResult<TSource>.FromError(errors.Count > 1 ? new AggregateException(errors) : errors[0]);

            return ParseResult<TSource>.FromResult(m_option);
        }
    }
}
