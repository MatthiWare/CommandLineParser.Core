using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using MatthiWare.CommandLine.Abstractions;
using MatthiWare.CommandLine.Abstractions.Models;
using MatthiWare.CommandLine.Abstractions.Parsing;
using Microsoft.Extensions.Logging;

namespace MatthiWare.CommandLine.Core
{
    [DebuggerDisplay("Cmd Option {ShortName ?? LongName ?? m_selector.ToString()}, Req: {IsRequired}, HasDefault: {HasDefault}")]
    internal abstract class CommandLineOptionBase : IParser, ICommandLineOption
    {
        private readonly object m_source;
        private readonly LambdaExpression m_selector;
        private readonly ILogger logger;
        private object m_defaultValue = null;
        protected readonly CommandLineParserOptions m_parserOptions;
        private Delegate m_translator = null;

        public CommandLineOptionBase(CommandLineParserOptions parserOptions, object source, LambdaExpression selector, ICommandLineArgumentResolver resolver, ILogger logger)
        {
            m_parserOptions = parserOptions ?? throw new ArgumentNullException(nameof(source));
            m_source = source ?? throw new ArgumentNullException(nameof(source));
            m_selector = selector ?? throw new ArgumentNullException(nameof(selector));
            Resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
        public bool HasShortName => !string.IsNullOrWhiteSpace(ShortName) && !ShortName.Equals(m_parserOptions.PrefixShortOption);
        public bool HasLongName => !string.IsNullOrWhiteSpace(LongName) && !LongName.Equals(m_parserOptions.PrefixLongOption);

        public bool AutoExecute { get; protected set; }

        public int? Order { get; protected set; }

        public void UseDefault() => AssignValue(DefaultValue);

        public bool CanParse(ArgumentModel model) => Resolver.CanResolve(model);

        public void Parse(ArgumentModel model) => AssignValue(Resolver.Resolve(model));

        private void AssignValue(object value)
        {
            var property = (PropertyInfo)((MemberExpression)m_selector.Body).Member;
            var actualValue = m_translator?.DynamicInvoke(value) ?? value;

            var key = $"{property.DeclaringType.FullName}.{property.Name}";
            logger.LogDebug("Option '{OptionName}' ({key}) value assigned '{value}'", ShortName, key, actualValue);

            property.SetValue(m_source, actualValue, null);
        }

        protected void SetTranslator(Delegate @delegate) => m_translator = @delegate;

        public bool CheckOptionNotFound() => IsRequired && !HasDefault;

        public bool ShouldUseDefault(bool found, ArgumentModel model)
            => (found && ShouldUseDefaultWhenParsingFails(model)) || (!found && ShouldUseDefaultWhenNoValueProvidedButDefaultValueIsSpecified(model));

        private bool ShouldUseDefaultWhenParsingFails(ArgumentModel model)
            => !CanParse(model) && HasDefault;

        private bool ShouldUseDefaultWhenNoValueProvidedButDefaultValueIsSpecified(ArgumentModel model)
            => (model is null || !model.HasValue) && HasDefault;

        public override string ToString() => m_selector.ToString();
    }
}
