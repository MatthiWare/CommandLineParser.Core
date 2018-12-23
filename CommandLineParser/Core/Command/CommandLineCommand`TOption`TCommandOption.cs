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
        ICommandBuilder<TOption>,
        ICommandConfigurationBuilder,
        ICommandConfigurationBuilder<TCommandOption>,
        IOptionConfigurator<TCommandOption>
        where TOption : class
        where TCommandOption : class, new()
    {
        private readonly TCommandOption m_commandOption;
        private readonly TOption m_baseOption;
        private readonly IArgumentResolverFactory m_resolverFactory;
        private readonly CommandLineParserOptions m_parserOptions;

        private Action<TOption> m_executor;
        private Action<TOption, TCommandOption> m_executor2;

        public CommandLineCommand(CommandLineParserOptions parserOptions, IArgumentResolverFactory resolverFactory, TOption option)
        {
            m_parserOptions = parserOptions;
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
            var option = new CommandLineOption(m_parserOptions, m_commandOption, selector, m_resolverFactory);

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
                    errors.Add(new OptionNotFoundException(m_parserOptions, option));

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

        ICommandBuilder<TOption> ICommandBuilder<TOption>.Description(string description)
        {
            Description = description;

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

        ICommandConfigurationBuilder ICommandConfigurationBuilder.Description(string description)
        {
            Description = description;

            return this;
        }

        public ICommandBuilder<TOption, TCommandOption> InvokeCommand(bool invoke)
        {
            AutoExecute = invoke;

            return this;
        }

        ICommandBuilder<TOption, TCommandOption> ICommandBuilder<TOption, TCommandOption>.Description(string help)
        {
            Description = help;

            return this;
        }

        ICommandBuilder<TOption, TCommandOption> ICommandBuilder<TOption, TCommandOption>.Name(string name)
        {
            Name = name;

            return this;
        }

        ICommandConfigurationBuilder ICommandConfigurationBuilder.Name(string name)
        {
            Name = name;

            return this;
        }

        ICommandBuilder<TOption> ICommandBuilder<TOption>.Name(string name)
        {
            Name = name;

            return this;
        }

        ICommandConfigurationBuilder<TCommandOption> ICommandConfigurationBuilder<TCommandOption>.Required(bool required)
        {
            IsRequired = required;

            return this;
        }

        ICommandConfigurationBuilder<TCommandOption> ICommandConfigurationBuilder<TCommandOption>.Description(string description)
        {
            Description = description;

            return this;
        }

        ICommandConfigurationBuilder<TCommandOption> ICommandConfigurationBuilder<TCommandOption>.Name(string name)
        {
            Name = name;

            return this;
        }
    }
}
