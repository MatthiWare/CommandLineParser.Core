using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using MatthiWare.CommandLine.Abstractions;
using MatthiWare.CommandLine.Abstractions.Parsing;
using MatthiWare.CommandLine.Core;
using MatthiWare.CommandLine.Core.Parsing;

[assembly: InternalsVisibleTo("CommandLineParser.Tests")]

namespace MatthiWare.CommandLine
{
    public sealed class CommandLineParser<TSource> : ICommandLineParser<TSource> where TSource : class, new()
    {
        private readonly TSource m_option;
        private readonly List<ICommandLineArgumentOption> m_options;
        private readonly IResolverFactory resolverFactory;

        public IReadOnlyList<ICommandLineArgumentOption> Options => m_options.AsReadOnly();

        public IResolverFactory ResolverFactory => throw new NotImplementedException();

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
            resolverFactory = new ResolverFactory();
        }

        public IOptionBuilder<TProperty> Configure<TProperty>(Expression<Func<TSource, TProperty>> selector)
        {
            return ConfigureInternal(selector);
        }

        private IOptionBuilder<TProperty> ConfigureInternal<TProperty>(Expression<Func<TSource, TProperty>> selector)
        {
            var option = new CommandLineArgumentOption<TProperty>();
            var builder = new OptionBuilder<TSource, TProperty>(m_option, option, selector);

            m_options.Add(option);

            return builder;
        }

        public IParserResult<TSource> Parse(string[] args)
        {
            throw new NotImplementedException();
        }
    }
}
