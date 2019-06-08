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
        public CommandLineOption(CommandLineParserOptions parserOptions, object source, LambdaExpression selector, IArgumentResolverFactory resolver)
            : base (parserOptions, source, selector, resolver)
        {
        }

        public IOptionBuilder Default(object defaultValue)
        {
            DefaultValue = defaultValue;

            return this;
        }

        IOptionBuilder IOptionBuilder.Description(string help)
        {
            Description = help;

            return this;
        }

        public IOptionBuilder Name(string shortName, string longName)
        {
            LongName = $"{m_parserOptions.PrefixLongOption}{longName}";
            ShortName = $"{m_parserOptions.PrefixShortOption}{shortName}";

            return this;
        }

        public IOptionBuilder Required(bool required = true)
        {
            IsRequired = required;

            return this;
        }

        public IOptionBuilder Name(string shortName)
        {
            ShortName = $"{m_parserOptions.PrefixShortOption}{shortName}";
            LongName = $"{m_parserOptions.PrefixLongOption}{shortName}";

            return this;
        }
        public IOptionBuilder Transform(Expression<Func<object, object>> transformation)
        {
            SetTranslator(transformation.Compile());

            return this;
        }
    }
}