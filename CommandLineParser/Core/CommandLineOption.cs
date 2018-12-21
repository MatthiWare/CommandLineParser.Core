using System;
using System.Linq.Expressions;
using System.Reflection;

using MatthiWare.CommandLine.Abstractions;
using MatthiWare.CommandLine.Abstractions.Models;
using MatthiWare.CommandLine.Abstractions.Parsing;

namespace MatthiWare.CommandLine.Core
{
    internal class CommandLineOption :
        CommandLineOptionBase,
        IOptionBuilder
    {
        private readonly object m_source;
        private readonly LambdaExpression m_selector;
        private object m_defaultValue = null;
        private readonly IArgumentResolverFactory m_resolverFactory;
        private readonly CommandLineParserOptions m_parserOptions;

        private ICommandLineArgumentResolver m_resolver;

        public CommandLineOption(CommandLineParserOptions parserOptions, object source, LambdaExpression selector, IArgumentResolverFactory resolver)
        {
            m_parserOptions = parserOptions;
            m_source = source ?? throw new ArgumentNullException(nameof(source));
            m_selector = selector ?? throw new ArgumentNullException(nameof(selector));
            m_resolverFactory = resolver ?? throw new ArgumentNullException(nameof(resolver));
        }

        public ICommandLineArgumentResolver Resolver
        {
            get
            {
                if (m_resolver == null)
                    m_resolver = m_resolverFactory.CreateResolver(m_selector.ReturnType);

                return m_resolver;
            }
        }

        public object DefaultValue
        {
            get => m_defaultValue;
            set
            {
                HasDefault = true;
                m_defaultValue = value;
            }
        }

        public override void UseDefault()
            => AssignValue(DefaultValue);

        public override bool CanParse(ArgumentModel model)
            => Resolver.CanResolve(model);

        public override void Parse(ArgumentModel model)
            => AssignValue(Resolver.Resolve(model));

        IOptionBuilder IOptionBuilder.Default(object defaultValue)
        {
            DefaultValue = defaultValue;

            return this;
        }

        IOptionBuilder IOptionBuilder.Description(string help)
        {
            Description = help;

            return this;
        }

        IOptionBuilder IOptionBuilder.Name(string shortName, string longName)
        {
            LongName = $"{m_parserOptions.PrefixLongOption}{longName}"; ;
            ShortName = $"{m_parserOptions.PrefixShortOption}{shortName}"; ;

            return this;
        }

        IOptionBuilder IOptionBuilder.Required(bool required = true)
        {
            IsRequired = required;

            return this;
        }

        IOptionBuilder IOptionBuilder.Name(string shortName)
        {
            ShortName = $"{m_parserOptions.PrefixShortOption}{shortName}";

            return this;
        }

        private void AssignValue(object value)
        {
            var property = (PropertyInfo)((MemberExpression)m_selector.Body).Member;
            property.SetValue(m_source, value, null);
        }
    }
}