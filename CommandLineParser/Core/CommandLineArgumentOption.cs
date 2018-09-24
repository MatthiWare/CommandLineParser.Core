using System;
using System.Linq.Expressions;
using System.Reflection;
using MatthiWare.CommandLine.Abstractions;
using MatthiWare.CommandLine.Abstractions.Models;
using MatthiWare.CommandLine.Abstractions.Parsing;

namespace MatthiWare.CommandLine.Core
{
    internal class CommandLineArgumentOption<TSource, TProperty> :
        ICommandLineArgumentOption<TProperty>,
        IParser,
        IOptionBuilder<TProperty> where TSource : class
    {
        private readonly TSource source;
        private readonly Expression<Func<TSource, TProperty>> selector;
        private TProperty m_defaultValue = default(TProperty);
        private readonly ICommandLineArgumentResolver<TProperty> resolver;

        public CommandLineArgumentOption(TSource source, Expression<Func<TSource, TProperty>> selector, ICommandLineArgumentResolver<TProperty> resolver)
        {
            this.source = source ?? throw new ArgumentNullException(nameof(source));
            this.selector = selector ?? throw new ArgumentNullException(nameof(selector));
            this.resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
        }

        public TProperty DefaultValue
        {
            get => m_defaultValue;
            set
            {
                HasDefault = true;
                m_defaultValue = value;
            }
        }

        public string ShortName { get; set; }
        public string LongName { get; set; }
        public string HelpText { get; set; }
        public bool IsRequired { get; set; }
        public bool HasDefault { get; private set; }

        public bool HasShortName => !string.IsNullOrWhiteSpace(ShortName);

        public bool HasLongName => !string.IsNullOrWhiteSpace(LongName);

        public void Parse(ArgumentModel model)
        {
            if (!resolver.CanResolve(model)) throw new ArgumentException("Invalid argument option", nameof(model));


        }

        IOptionBuilder<TProperty> IOptionBuilder<TProperty>.Default(TProperty defaultValue)
        {
            DefaultValue = defaultValue;

            return this;
        }

        IOptionBuilder<TProperty> IOptionBuilder<TProperty>.HelpText(string help)
        {
            HelpText = help;

            return this;
        }

        IOptionBuilder<TProperty> IOptionBuilder<TProperty>.LongName(string longName)
        {
            LongName = longName;

            return this;
        }

        IOptionBuilder<TProperty> IOptionBuilder<TProperty>.Required(bool required = true)
        {
            IsRequired = required;

            return this;
        }

        IOptionBuilder<TProperty> IOptionBuilder<TProperty>.ShortName(string shortName)
        {
            ShortName = shortName;

            return this;
        }

        private void AssignValue(TProperty value)
        {
            var property = (PropertyInfo)((MemberExpression)selector.Body).Member;
            property.SetValue(source, value, null);
        }
    }
}