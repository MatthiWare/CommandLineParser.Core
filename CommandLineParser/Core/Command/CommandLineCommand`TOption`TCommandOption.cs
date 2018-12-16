using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using MatthiWare.CommandLine.Abstractions;
using MatthiWare.CommandLine.Abstractions.Command;
using MatthiWare.CommandLine.Abstractions.Models;
using MatthiWare.CommandLine.Abstractions.Parsing;
using MatthiWare.CommandLine.Abstractions.Parsing.Command;
using MatthiWare.CommandLine.Core.Exceptions;

namespace MatthiWare.CommandLine.Core.Command
{
    internal class CommandLineCommand<TOption, TCommandOption> :
        CommandLineCommandBase,
        ICommandBuilder<TOption, TCommandOption>,
        ICommandConfigurationBuilder,
        ICommandBuilder<TOption>,
        IOptionConfigurator<TCommandOption>
        where TOption : class
        where TCommandOption : class, new()
    {
        private readonly TCommandOption m_commandOption;
        private readonly TOption m_baseOption;
        private readonly IArgumentResolverFactory m_resolverFactory;

        private Action<TOption> m_executor;
        private Action<TOption, TCommandOption> m_executor2;

        public CommandLineCommand(IArgumentResolverFactory resolverFactory, TOption option)
        {
            m_commandOption = new TCommandOption();

            m_resolverFactory = resolverFactory;
            m_baseOption = option;
        }

        public override void Execute()
        {
            m_executor2?.Invoke(m_baseOption, m_commandOption);
            m_executor?.Invoke(m_baseOption);
        }

        public IOptionBuilder Configure<TProperty>(Expression<Func<TCommandOption, TProperty>> selector)
        {
            var option = new CommandLineOption(m_commandOption, selector, m_resolverFactory);

            m_options.Add(option);

            return option;
        }

        public override ICommandParserResult Parse(IArgumentManager argumentManager)
        {
            var result = new CommandParserResult(this);
            var errors = new List<Exception>();

            foreach (var option in m_options)
            {
                if (!argumentManager.TryGetValue(option, out ArgumentModel model) && option.IsRequired)
                {
                    errors.Add(new OptionNotFoundException(option));

                    continue;
                }
                else if (!model.HasValue && option.HasDefault)
                {
                    option.UseDefault();

                    continue;
                }
                else if (!option.CanParse(model))
                {
                    errors.Add(new OptionParseException(option, model));

                    continue;
                }

                option.Parse(model);
            }

            result.MergeResult(errors);

            return result;
        }

        public ICommandBuilder<TOption, TCommandOption> Required(bool required = true)
        {
            IsRequired = required;

            return this;
        }

        public ICommandBuilder<TOption, TCommandOption> Name(string shortName)
        {
            ShortName = shortName;

            return this;
        }

        public ICommandBuilder<TOption, TCommandOption> Name(string shortName, string longName)
        {
            ShortName = shortName;
            LongName = longName;

            return this;
        }

        public ICommandBuilder<TOption, TCommandOption> OnExecuting(Action<TOption> action)
        {
            m_executor = action;

            return this;
        }

        public ICommandBuilder<TOption, TCommandOption> OnExecuting(Action<TOption, TCommandOption> action)
        {
            m_executor2 = action;

            return this;
        }

        ICommandBuilder<TOption> ICommandBuilder<TOption>.InvokeCommand(bool invoke)
        {
            AutoExecute = invoke;

            return this;
        }

        ICommandBuilder<TOption> ICommandBuilder<TOption>.Required(bool required)
        {
            IsRequired = required;

            return this;
        }

        ICommandBuilder<TOption> ICommandBuilder<TOption>.HelpText(string help)
        {
            HelpText = help;

            return this;
        }

        ICommandBuilder<TOption> ICommandBuilder<TOption>.Name(string shortName)
        {
            ShortName = shortName;

            return this;
        }

        ICommandBuilder<TOption> ICommandBuilder<TOption>.Name(string shortName, string longName)
        {
            ShortName = shortName;
            LongName = longName;

            return this;
        }

        ICommandBuilder<TOption> ICommandBuilder<TOption>.OnExecuting(Action<TOption> action)
        {
            m_executor = action;

            return this;
        }

        ICommandConfigurationBuilder ICommandConfigurationBuilder.Required(bool required)
        {
            IsRequired = required;

            return this;
        }

        ICommandConfigurationBuilder ICommandConfigurationBuilder.HelpText(string help)
        {
            HelpText = help;

            return this;
        }

        ICommandConfigurationBuilder ICommandConfigurationBuilder.Name(string shortName)
        {
            ShortName = shortName;

            return this;
        }

        ICommandConfigurationBuilder ICommandConfigurationBuilder.Name(string shortName, string longName)
        {
            ShortName = shortName;
            LongName = longName;

            return this;
        }

        public ICommandBuilder<TOption, TCommandOption> InvokeCommand(bool invoke)
        {
            AutoExecute = invoke;

            return this;
        }

        ICommandBuilder<TOption, TCommandOption> ICommandBuilder<TOption, TCommandOption>.HelpText(string help)
        {
            HelpText = help;

            return this;
        }
    }
}
