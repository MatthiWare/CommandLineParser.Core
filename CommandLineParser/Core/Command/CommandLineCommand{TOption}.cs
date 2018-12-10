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
    internal class CommandLineCommand<TOption> :
        CommandLineCommandBase,
        ICommandBuilder<TOption>,
        where TOption : class
    {
        private readonly TOption m_option;
        private readonly IResolverFactory m_resolverFactory;
        private Action<TOption> m_executor;

        public CommandLineCommand(IResolverFactory resolverFactory)
        {
            this.m_resolverFactory = resolverFactory;
        }

        public override void Execute()
        {
            m_executor?.Invoke(m_option);
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

        public CommandLineCommand<TOption> OnExecuting(Action<TOption> action)
        {
            m_genericExecutor = action;

            return this;
        }

        public CommandLineCommand<TOption> Required(bool required = true)
        {
            IsRequired = required;

            return this;
        }

        CommandLineCommand<TOption> ICommandBuilder<CommandLineCommand<TOption>>.HelpText(string help)
        {
            throw new NotImplementedException();
        }

        public CommandLineCommand<TOption> Name(string shortName)
        {
            throw new NotImplementedException();
        }

        public CommandLineCommand<TOption> Name(string shortName, string longName)
        {
            throw new NotImplementedException();
        }

        public CommandLineCommand<TOption> OnExecuting(Action action)
        {
            m_executor = action;

            return this;
        }
    }
}
