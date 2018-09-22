using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using MatthiWare.CommandLine.Abstractions;

namespace MatthiWare.CommandLine.Core
{
    internal sealed class OptionBuilder<TSource, TProperty> : IOptionBuilder<TProperty> where TSource : class
    {
        private readonly TSource source;
        private readonly ICommandLineArgumentOption<TProperty> option;
        private readonly Expression<Func<TSource, TProperty>> selector;

        public OptionBuilder(TSource source, ICommandLineArgumentOption<TProperty> option, Expression<Func<TSource, TProperty>> selector)
        {
            this.source = source ?? throw new ArgumentNullException(nameof(source));
            this.option = option ?? throw new ArgumentNullException(nameof(option));
            this.selector = selector ?? throw new ArgumentNullException(nameof(selector));
        }

        public IOptionBuilder<TProperty> Default(TProperty defaultValue = default(TProperty))
        {
            option.DefaultValue = defaultValue;

            return this;
        }

        public IOptionBuilder<TProperty> HelpText(string help)
        {
            option.HelpText = help;

            return this;
        }

        public IOptionBuilder<TProperty> LongName(string longName)
        {
            option.LongName = longName;

            return this;
        }

        public IOptionBuilder<TProperty> Required(bool required = true)
        {
            option.IsRequired = required;

            return this;
        }

        public IOptionBuilder<TProperty> ShortName(string shortName)
        {
            option.ShortName = shortName;

            return this;
        }

        private void AssignValue(TProperty value)
        {
            var property = (PropertyInfo)((MemberExpression)selector.Body).Member;
            property.SetValue(source, value, null);
        }
    }
}
