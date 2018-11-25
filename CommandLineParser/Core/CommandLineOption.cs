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
        ICommandLineOption,
        IOptionBuilder
    {
        private readonly object source;
        private readonly LambdaExpression selector;
        private object m_defaultValue = null;
        private readonly ICommandLineArgumentResolver resolver;

        public CommandLineOption(object source, LambdaExpression selector, ICommandLineArgumentResolver resolver)
        {
            this.source = source ?? throw new ArgumentNullException(nameof(source));
            this.selector = selector ?? throw new ArgumentNullException(nameof(selector));
            this.resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
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
            => resolver.CanResolve(model);

        public override void Parse(ArgumentModel model)
            => AssignValue(resolver.Resolve(model));

        IOptionBuilder IOptionBuilder.Default(object defaultValue)
        {
            DefaultValue = defaultValue;

            return this;
        }

        IOptionBuilder IOptionBuilder.HelpText(string help)
        {
            HelpText = help;

            return this;
        }

        IOptionBuilder IOptionBuilder.Name(string shortName, string longName)
        {
            LongName = longName;
            ShortName = shortName;

            return this;
        }

        IOptionBuilder IOptionBuilder.Required(bool required = true)
        {
            IsRequired = required;

            return this;
        }

        IOptionBuilder IOptionBuilder.Name(string shortName)
        {
            ShortName = shortName;

            return this;
        }

        private void AssignValue(object value)
        {
            var property = (PropertyInfo)((MemberExpression)selector.Body).Member;
            property.SetValue(source, value, null);
        }
    }
}