using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using MatthiWare.CommandLine.Abstractions;
using MatthiWare.CommandLine.Abstractions.Models;
using MatthiWare.CommandLine.Abstractions.Parsing;

namespace MatthiWare.CommandLine.Core
{
    [DebuggerDisplay("Cmd Option {ShortName ?? LongName}, Req: {IsRequired}, HasDefault: {HasDefault}")]
    internal abstract class CommandLineOptionBase : IParser, ICommandLineOption
    {
        private readonly object m_source;
        private readonly LambdaExpression m_selector;
        private object m_defaultValue = null;
        protected readonly CommandLineParserOptions m_parserOptions;
        private Delegate m_translator = null;

        public CommandLineOptionBase(CommandLineParserOptions parserOptions, object source, LambdaExpression selector, ICommandLineArgumentResolver resolver)
        {
            m_parserOptions = parserOptions ?? throw new ArgumentNullException(nameof(source));
            m_source = source ?? throw new ArgumentNullException(nameof(source));
            m_selector = selector ?? throw new ArgumentNullException(nameof(selector));
            Resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
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

        public ICommandLineArgumentResolver Resolver { get; }

        public string ShortName { get; protected set; }
        public string LongName { get; protected set; }
        public string Description { get; protected set; }
        public bool IsRequired { get; protected set; }
        public bool HasDefault { get; protected set; }
        public bool HasShortName => !string.IsNullOrWhiteSpace(ShortName);
        public bool HasLongName => !string.IsNullOrWhiteSpace(LongName);

        public bool AutoExecute { get; protected set; }

        public void UseDefault() => AssignValue(DefaultValue);

        public bool CanParse(ArgumentModel model) => Resolver.CanResolve(model);

        public void Parse(ArgumentModel model) => AssignValue(Resolver.Resolve(model));

        private void AssignValue(object value)
        {
            var property = (PropertyInfo)((MemberExpression)m_selector.Body).Member;
            property.SetValue(m_source, m_translator?.DynamicInvoke(value) ?? value, null);
        }

        protected void SetTranslator(Delegate @delegate) => m_translator = @delegate;
    }
}
