using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using MatthiWare.CommandLine.Abstractions;
using MatthiWare.CommandLine.Abstractions.Command;
using MatthiWare.CommandLine.Abstractions.Models;
using MatthiWare.CommandLine.Abstractions.Parsing;
using MatthiWare.CommandLine.Core;
using MatthiWare.CommandLine.Core.Attributes;
using MatthiWare.CommandLine.Core.Command;
using MatthiWare.CommandLine.Core.Exceptions;
using MatthiWare.CommandLine.Core.Parsing;

[assembly: InternalsVisibleTo("CommandLineParser.Tests")]

namespace MatthiWare.CommandLine
{
    /// <summary>
    /// Command line parser
    /// </summary>
    /// <typeparam name="TOption">Options model</typeparam>
    public sealed class CommandLineParser<TOption> : ICommandLineParser<TOption>
        where TOption : class, new()
    {
        private readonly TOption m_option;
        private readonly Dictionary<string, CommandLineOptionBase> m_options;
        private readonly List<CommandLineCommandBase> m_commands;

        /// <summary>
        /// Read-only collection of options specified
        /// </summary>
        public IReadOnlyList<ICommandLineOption> Options => new ReadOnlyCollectionWrapper<string, CommandLineOptionBase>(m_options.Values);

        /// <summary>
        /// Factory to create resolvers for options
        /// </summary>
        public IResolverFactory ResolverFactory { get; }

        /// <summary>
        /// Read-only list of commands specified
        /// </summary>
        public IReadOnlyList<ICommandLineCommand> Commands => m_commands.AsReadOnly();

        /// <summary>
        /// Creates a new instance of the commandline parser
        /// </summary>
        public CommandLineParser()
        {
            m_option = new TOption();

            m_options = new Dictionary<string, CommandLineOptionBase>();
            m_commands = new List<CommandLineCommandBase>();

            ResolverFactory = new ResolverFactory();

            InitialzeModel();
        }

        /// <summary>
        /// Configures an option in the model
        /// </summary>
        /// <typeparam name="TProperty">Type of the property</typeparam>
        /// <param name="selector">Model property to configure</param>
        /// <returns><see cref="IOptionBuilder"/></returns>
        public IOptionBuilder Configure<TProperty>(Expression<Func<TOption, TProperty>> selector)
        {
            var memberInfo = ((MemberExpression)selector.Body).Member;
            var key = $"{memberInfo.DeclaringType.FullName}.{memberInfo.Name}";

            return ConfigureInternal(selector, key);
        }

        private IOptionBuilder ConfigureInternal(LambdaExpression selector, string key)
        {
            if (!m_options.ContainsKey(key))
            {
                var option = new CommandLineOption(m_option, selector, ResolverFactory);

                m_options.Add(key, option);
            }

            return m_options[key] as IOptionBuilder;
        }

        /// <summary>
        /// Parses the commandline arguments
        /// </summary>
        /// <param name="args">arguments from the commandline</param>
        /// <returns>The result of the parsing, <see cref="IParserResult{TResult}"/></returns>
        public IParserResult<TOption> Parse(string[] args)
        {
            var errors = new List<Exception>();

            var result = new ParseResult<TOption>();

            var argumentManager = new ArgumentManager(args, m_commands, m_options.Values);

            ParseCommands(errors, result, argumentManager);

            ParseOptions(errors, result, argumentManager);

            result.MergeResult(errors);

            result.MergeResult(m_option);

            AutoExecuteCommands(result);

            return result;
        }

        private void AutoExecuteCommands(ParseResult<TOption> result)
        {
            if (result.HasErrors) return;

            foreach (var cmd in result.CommandResults.Where(r => r.Command.AutoExecute))
                cmd.ExecuteCommand();
        }

        private void ParseCommands(IList<Exception> errors, ParseResult<TOption> result, IArgumentManager argumentManager)
        {
            foreach (var cmd in m_commands)
            {
                if (!argumentManager.TryGetValue(cmd, out ArgumentModel model) && cmd.IsRequired)
                {
                    errors.Add(new CommandNotFoundException(cmd));

                    continue;
                }

                var cmdParseResult = cmd.Parse(argumentManager);

                if (cmdParseResult.HasErrors)
                    errors.Add(new CommandParseException(cmd, cmdParseResult.Error));

                result.MergeResult(cmdParseResult);
            }
        }

        private void ParseOptions(IList<Exception> errors, ParseResult<TOption> result, IArgumentManager argumentManager)
        {
            foreach (var o in m_options)
            {
                var option = o.Value;

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
        }

        /// <summary>
        /// Adds a command to the parser
        /// </summary>
        /// <typeparam name="TCommandOption">Options model for the command</typeparam>
        /// <returns>Builder for the command, <see cref="ICommandBuilder{TOption,TCommandOption}"/></returns>
        public ICommandBuilder<TOption, TCommandOption> AddCommand<TCommandOption>() where TCommandOption : class, new()
        {
            var command = new CommandLineCommand<TOption, TCommandOption>(ResolverFactory, () => m_option);

            m_commands.Add(command);

            return command;
        }

        /// <summary>
        /// Adds a command to the parser
        /// </summary>
        /// <typeparam name="TCommandOption">Command type, must be inherit <see cref="Command{TOptions, TCommandOptions}"/></typeparam>
        public void AddCommand<TCommand, TCommandOption>()
            where TCommand : Command<TOption, TCommandOption>, new()
            where TCommandOption : class, new()
        {
            var cmdConfigurator = new TCommand();

            var command = new CommandLineCommand<TOption, TCommandOption>(ResolverFactory, () => m_option);

            cmdConfigurator.OnConfigure(command);
        }

        /// <summary>
        /// Adds a command to the parser
        /// </summary>
        /// <returns>Builder for the command, <see cref="ICommandBuilder{TOption}"/></returns>
        public ICommandBuilder<TOption> AddCommand()
        {
            var command = new CommandLineCommand<TOption>(ResolverFactory);

            m_commands.Add(command);

            return command;
        }

        /// <summary>
        /// Initializes the model class with the attributes specified.
        /// </summary>
        private void InitialzeModel()
        {
            var properties = typeof(TOption).GetProperties();

            foreach (var propInfo in properties)
            {
                var attributes = propInfo.GetCustomAttributes(true);

                var lambda = GetLambdaExpression(propInfo, out string key);

                var actions = new List<Action>(4);
                bool ignoreSet = false;

                foreach (var attribute in attributes)
                {
                    if (ignoreSet) break;

                    switch (attribute)
                    {
                        // Ignore has been set, skip all the other attributes and DO NOT execute the action list.
                        case IgnoreAttribute ignore:
                            ignoreSet = true;
                            continue;
                        case RequiredAttribute required:
                            actions.Add(() => ConfigureInternal(lambda, key).Required(required.Required));
                            break;
                        case DefaultValueAttribute defaultValue:
                            actions.Add(() => ConfigureInternal(lambda, key).Default(defaultValue.DefaultValue));
                            break;
                        case HelpTextAttribute helpText:
                            actions.Add(() => ConfigureInternal(lambda, key).HelpText(helpText.HelpText));
                            break;
                        case NameAttribute name:
                            actions.Add(() => ConfigureInternal(lambda, key).Name(name.ShortName, name.LongName));
                            break;
                    }
                }

                if (ignoreSet) continue; // Ignore the configured actions for this option.

                foreach (var action in actions)
                    action();
            }

            LambdaExpression GetLambdaExpression(PropertyInfo propInfo, out string key)
            {
                var entityType = propInfo.DeclaringType;
                var propType = propInfo.PropertyType;
                var parameter = Expression.Parameter(entityType, entityType.FullName);
                var property = Expression.Property(parameter, propInfo);
                var funcType = typeof(Func<,>).MakeGenericType(entityType, propType);

                key = $"{entityType.ToString()}.{propInfo.Name}";

                return Expression.Lambda(funcType, property, parameter);
            }
        }


    }
}
