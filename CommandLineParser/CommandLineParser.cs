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
    public sealed class CommandLineParser<TSource> : ICommandLineParser<TSource> where TSource : class, new()
    {
        private readonly TSource m_option;
        private readonly Dictionary<string, CommandLineOptionBase> m_options;
        private readonly List<CommandLineCommandBase> m_commands;

        public IReadOnlyList<ICommandLineOption> Options => new ReadOnlyCollectionWrapper<string, CommandLineOptionBase>(m_options.Values);

        public IResolverFactory ResolverFactory { get; }

        public IReadOnlyList<ICommandLineCommand> Commands => m_commands.AsReadOnly();

        public CommandLineParser()
        {
            m_option = new TSource();

            m_options = new Dictionary<string, CommandLineOptionBase>();
            m_commands = new List<CommandLineCommandBase>();

            ResolverFactory = new ResolverFactory();

            InitialzeModel();
        }

        public IOptionBuilder Configure<TProperty>(Expression<Func<TSource, TProperty>> selector)
        {
            var memberInfo = ((MemberExpression)selector.Body).Member;
            var key = $"{memberInfo.DeclaringType.FullName}.{memberInfo.Name}";

            return ConfigureInternal(selector, key);
        }

        private IOptionBuilder ConfigureInternal(LambdaExpression selector, string key)
        {
            if (!m_options.ContainsKey(key))
            {
                var option = new CommandLineOption(m_option, selector, ResolverFactory.CreateResolver(selector.ReturnType));

                m_options.Add(key, option);
            }

            return m_options[key] as IOptionBuilder;
        }

        public IParserResult<TSource> Parse(string[] args)
        {
            var errors = new List<Exception>();

            var result = new ParseResult<TSource>();

            var argumentManager = new ArgumentManager(args, m_commands, m_options.Values);

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

            if (errors.Any())
                result.MergeResult(errors);

            result.MergeResult(m_option);

            return result;
        }

        public ICommandBuilder<TCommandOption> AddCommand<TCommandOption>() where TCommandOption : class, new()
        {
            var command = new CommandLineCommand<TCommandOption>(ResolverFactory);

            m_commands.Add(command);

            return command;
        }

        private void InitialzeModel()
        {
            var properties = typeof(TSource).GetProperties();

            foreach (var propInfo in properties)
            {
                var attributes = propInfo.GetCustomAttributes(true);

                var lambda = GetLambdaExpression(propInfo, out string key);

                foreach (var attribute in attributes)
                {
                    switch (attribute)
                    {
                        case RequiredAttribute required:
                            ConfigureInternal(lambda, key).Required(required.Required);

                            break;
                        case DefaultValueAttribute defaultValue:
                            ConfigureInternal(lambda, key).Default(defaultValue.DefaultValue);

                            break;
                        case HelpTextAttribute helpText:
                            ConfigureInternal(lambda, key).HelpText(helpText.HelpText);
                            break;
                        case NameAttribute name:
                            ConfigureInternal(lambda, key).Name(name.ShortName, name.LongName);

                            break;
                    }
                }
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
