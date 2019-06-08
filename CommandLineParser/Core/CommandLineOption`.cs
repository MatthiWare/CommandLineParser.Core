using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using MatthiWare.CommandLine.Abstractions;
using MatthiWare.CommandLine.Abstractions.Models;
using MatthiWare.CommandLine.Abstractions.Parsing;

namespace MatthiWare.CommandLine.Core
{
    internal class CommandLineOption<TOption> : CommandLineOptionBase, IOptionBuilder<TOption>
    {
        public CommandLineOption(CommandLineParserOptions parserOptions, object source, LambdaExpression selector, IArgumentResolverFactory resolver)
             : base(parserOptions, source, selector, resolver)
        {
        }

        public IOptionBuilder<TOption> Required(bool required = true)
        {
            IsRequired = required;

            return this;
        }

        IOptionBuilder<TOption> IOptionBuilder<TOption>.Description(string description)
        {
            Description = description;

            return this;
        }

        public IOptionBuilder<TOption> Default(TOption defaultValue)
        {
            DefaultValue = defaultValue;

            return this;
        }

        public IOptionBuilder<TOption> Name(string shortName)
        {
            LongName = $"{m_parserOptions.PrefixLongOption}{shortName}";
            ShortName = $"{m_parserOptions.PrefixShortOption}{shortName}";

            return this;
        }

        public IOptionBuilder<TOption> Name(string shortName, string longName)
        {
            LongName = $"{m_parserOptions.PrefixLongOption}{longName}";
            ShortName = $"{m_parserOptions.PrefixShortOption}{shortName}";

            return this;
        }

        public IOptionBuilder<TOption> Transform(Expression<Func<TOption, TOption>> transformation)
        {
            SetTranslator(transformation.Compile());

            return this;
        }

        IOptionBuilder IOptionBuilder.Required(bool required)
        {
            IsRequired = required;

            return this;
        }

        IOptionBuilder IOptionBuilder.Description(string description)
        {
            Description = description;

            return this;
        }

        IOptionBuilder IOptionBuilder.Default(object defaultValue)
        {
            DefaultValue = defaultValue;

            return this;
        }

        IOptionBuilder IOptionBuilder.Name(string shortName)
        {
            LongName = $"{m_parserOptions.PrefixLongOption}{shortName}";
            ShortName = $"{m_parserOptions.PrefixShortOption}{shortName}";

            return this;
        }

        IOptionBuilder IOptionBuilder.Name(string shortName, string longName)
        {
            LongName = $"{m_parserOptions.PrefixLongOption}{longName}";
            ShortName = $"{m_parserOptions.PrefixShortOption}{shortName}";

            return this;
        }

        IOptionBuilder IOptionBuilder.Transform(Expression<Func<object, object>> transformation)
        {
            SetTranslator(transformation.Compile());

            return this;
        }
    }
}
