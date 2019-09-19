using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using MatthiWare.CommandLine.Abstractions;
using MatthiWare.CommandLine.Abstractions.Command;
using MatthiWare.CommandLine.Abstractions.Models;
using MatthiWare.CommandLine.Abstractions.Parsing;
using MatthiWare.CommandLine.Abstractions.Parsing.Command;
using MatthiWare.CommandLine.Abstractions.Validations;
using MatthiWare.CommandLine.Core.Attributes;
using MatthiWare.CommandLine.Core.Exceptions;
using MatthiWare.CommandLine.Core.Parsing.Command;
using MatthiWare.CommandLine.Core.Utils;

namespace MatthiWare.CommandLine.Core.Command
{
    internal class CommandLineCommand<TOption, TCommandOption> :
        CommandLineCommandBase,
        ICommandBuilder<TOption, TCommandOption>,
        ICommandBuilder<TOption>,
        ICommandConfigurationBuilder,
        ICommandConfigurationBuilder<TCommandOption>,
        IOptionConfigurator<TCommandOption>
        where TOption : class, new()
        where TCommandOption : class, new()
    {
        private readonly TCommandOption m_commandOption;
        private readonly TOption m_baseOption;
        private readonly IArgumentResolverFactory m_resolverFactory;
        private readonly IContainerResolver m_containerResolver;
        private readonly CommandLineParserOptions m_parserOptions;
        private readonly IValidatorsContainer m_validators;

        private Action m_executor;
        private Action<TOption> m_executor1;
        private Action<TOption, TCommandOption> m_executor2;

        private readonly string m_helpOptionName;
        private readonly string m_helpOptionNameLong;

        public CommandLineCommand(CommandLineParserOptions parserOptions, IArgumentResolverFactory resolverFactory, IContainerResolver containerResolver, TOption option, IValidatorsContainer validators)
        {
            m_parserOptions = parserOptions;
            m_commandOption = new TCommandOption();

            m_validators = validators;
            m_containerResolver = containerResolver;
            m_resolverFactory = resolverFactory;
            m_baseOption = option;

            if (m_parserOptions.EnableHelpOption)
            {
                var tokens = m_parserOptions.HelpOptionName.Split('|');

                if (tokens.Length > 1)
                {
                    m_helpOptionName = $"{m_parserOptions.PrefixShortOption}{tokens[0]}";
                    m_helpOptionNameLong = $"{m_parserOptions.PrefixLongOption}{tokens[1]}";
                }
                else
                {
                    m_helpOptionName = $"{m_parserOptions.PrefixLongOption}{tokens[0]}";
                    m_helpOptionNameLong = null;
                }
            }

            InitialzeModel();
        }

        public override void Execute()
        {
            m_executor2?.Invoke(m_baseOption, m_commandOption);
            m_executor1?.Invoke(m_baseOption);
            m_executor?.Invoke();
        }

        public IOptionBuilder<TProperty> Configure<TProperty>(Expression<Func<TCommandOption, TProperty>> selector)
        {
            var memberInfo = ((MemberExpression)selector.Body).Member;
            var key = $"{memberInfo.DeclaringType.FullName}.{memberInfo.Name}";

            return ConfigureInternal<TProperty>(selector, key);
        }

        private IOptionBuilder<T> ConfigureInternal<T>(LambdaExpression selector, string key)
        {
            if (!m_options.ContainsKey(key))
            {
                var option = new CommandLineOption<T>(m_parserOptions, m_commandOption, selector, m_resolverFactory);

                m_options.Add(key, option);
            }

            return m_options[key] as IOptionBuilder<T>;
        }

        public override ICommandParserResult Parse(IArgumentManager argumentManager)
        {
            var result = new CommandParserResult(this);
            var errors = new List<Exception>();

            ParseCommands(errors, result, argumentManager);

            ParseOptions(errors, result, argumentManager);

            Validate(m_commandOption, errors);

            result.MergeResult(errors);

            return result;
        }

        private void ParseOptions(IList<Exception> errors, CommandParserResult result, IArgumentManager argumentManager)
        {
            foreach (var o in m_options)
            {
                try
                {
                    var option = o.Value;
                    bool found = argumentManager.TryGetValue(option, out ArgumentModel model);

                    if (found && HelpRequested(result, option, model))
                    {
                        break;
                    }
                    else if (!found && option.IsRequired && !option.HasDefault)
                    {
                        throw new OptionNotFoundException(option);
                    }
                    else if ((!found && !model.HasValue && option.HasDefault) ||
                        (found && !option.CanParse(model) && option.HasDefault))
                    {
                        option.UseDefault();

                        continue;
                    }
                    else if (found && !option.CanParse(model))
                    {
                        throw new OptionParseException(option, model);
                    }

                    option.Parse(model);
                }
                catch (Exception e)
                {
                    errors.Add(e);
                }
            }
        }

        private void ParseCommands(IList<Exception> errors, CommandParserResult result, IArgumentManager argumentManager)
        {
            foreach (var cmd in m_commands)
            {
                try
                {
                    if (!argumentManager.TryGetValue(cmd, out ArgumentModel model))
                    {
                        result.MergeResult(new CommandNotFoundParserResult(cmd));

                        if (cmd.IsRequired)
                            throw new CommandNotFoundException(cmd);

                        continue;
                    }

                    var cmdParseResult = cmd.Parse(argumentManager);

                    if (cmdParseResult.HelpRequested)
                        break;

                    result.MergeResult(cmdParseResult);

                    if (cmdParseResult.HasErrors)
                        throw new CommandParseException(cmd, cmdParseResult.Errors);
                }
                catch (Exception ex)
                {
                    errors.Add(ex);
                }
            }
        }

        private bool HelpRequested(CommandParserResult result, CommandLineOptionBase option, ArgumentModel model)
        {
            if (!m_parserOptions.EnableHelpOption) return false;

            if (model.Key.Equals(m_helpOptionName, StringComparison.InvariantCultureIgnoreCase) ||
                model.Key.Equals(m_helpOptionNameLong, StringComparison.InvariantCultureIgnoreCase))
            {
                result.HelpRequestedFor = this;

                return true;
            }
            else if (model.HasValue &&
              (model.Value.Equals(m_helpOptionName, StringComparison.InvariantCultureIgnoreCase) ||
              model.Value.Equals(m_helpOptionNameLong, StringComparison.InvariantCultureIgnoreCase)))
            {
                result.HelpRequestedFor = option;

                return true;
            }

            return false;
        }

        private void Validate<T>(T @object, List<Exception> errors)
        {
            if (!m_validators.HasValidatorFor<T>())
                return;

            var results = m_validators.GetValidators<T>().Select(validator => validator.Validate(@object)).ToArray();

            foreach (var result in results)
            {
                if (result.IsValid)
                    continue;

                errors.Add(result.Error);
            }
        }

        public ICommandBuilder<TOption, TCommandOption> Required(bool required = true)
        {
            IsRequired = required;

            return this;
        }

        public ICommandBuilder<TOption, TCommandOption> OnExecuting(Action<TOption> action)
        {
            m_executor1 = action;

            return this;
        }

        public ICommandBuilder<TOption, TCommandOption> OnExecuting(Action action)
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
            m_executor1 = action;

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

        /// <summary>
        /// Initializes the model class with the attributes specified.
        /// </summary>
        private void InitialzeModel()
        {
            var properties = typeof(TCommandOption).GetProperties();

            foreach (var propInfo in properties)
            {
                var attributes = propInfo.GetCustomAttributes(true);

                var lambda = propInfo.GetLambdaExpression(out string key);

                var actions = new List<Action>(4);
                bool ignoreSet = false;

                var cfg = GetType().GetMethod(nameof(ConfigureInternal), BindingFlags.NonPublic | BindingFlags.Instance);

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
                            actions.Add(() => GetOption(cfg, propInfo, lambda, key).Required(required.Required));
                            break;
                        case DefaultValueAttribute defaultValue:
                            actions.Add(() => GetOption(cfg, propInfo, lambda, key).Default(defaultValue.DefaultValue));
                            break;
                        case DescriptionAttribute helpText:
                            actions.Add(() => GetOption(cfg, propInfo, lambda, key).Description(helpText.Description));
                            break;
                        case NameAttribute name:
                            actions.Add(() => GetOption(cfg, propInfo, lambda, key).Name(name.ShortName, name.LongName));
                            break;
                    }
                }

                if (ignoreSet) continue; // Ignore the configured actions for this option.

                if (propInfo.PropertyType.IsAssignableToGenericType(typeof(Command<>)))
                {
                    var genericTypes = propInfo.PropertyType.BaseType.GenericTypeArguments;
                    var method = GetType().GetMethods().First(m =>
                    {
                        return (m.Name == nameof(RegisterCommand) && m.IsGenericMethod && m.GetGenericArguments().Length == genericTypes.Length);
                    });
                    var registerCommand = genericTypes.Length > 1 ? method.MakeGenericMethod(propInfo.PropertyType, genericTypes[1]) : method.MakeGenericMethod(propInfo.PropertyType);

                    registerCommand.Invoke(this, null);
                }

                foreach (var action in actions)
                    action();
            }

            IOptionBuilder GetOption(MethodInfo method, PropertyInfo prop, LambdaExpression lambda, string key)
            {
                return method.InvokeGenericMethod(prop, this, lambda, key) as IOptionBuilder;
            }
        }

        /// <summary>
        /// Registers a command type
        /// </summary>
        /// <typeparam name="TCommandOption">Command type, must be inherit <see cref="Command{TOptions, TCommandOptions}"/></typeparam>
        public void RegisterCommand<TCommand>()
            where TCommand : Command<TCommandOption>
        {
            var cmdConfigurator = m_containerResolver.Resolve<TCommand>();

            var command = new CommandLineCommand<TCommandOption, object>(m_parserOptions, m_resolverFactory, m_containerResolver, m_commandOption, m_validators);

            cmdConfigurator.OnConfigure(command);

            command.OnExecuting((Action<TCommandOption>)cmdConfigurator.OnExecute);

            m_commands.Add(command);
        }

        /// <summary>
        /// Registers a command type
        /// </summary>
        /// <typeparam name="TCommand"></typeparam>
        /// <typeparam name="TCommandOption"></typeparam>
        public void RegisterCommand<TCommand, V>()
           where TCommand : Command<TOption, V>
           where V : class, new()
        {
            var cmdConfigurator = m_containerResolver.Resolve<TCommand>();

            var command = new CommandLineCommand<TOption, V>(m_parserOptions, m_resolverFactory, m_containerResolver, m_baseOption, m_validators);

            cmdConfigurator.OnConfigure(command);

            command.OnExecuting((Action<TOption, V>)cmdConfigurator.OnExecute);

            m_commands.Add(command);
        }

        ICommandConfigurationBuilder<TCommandOption> ICommandConfigurationBuilder<TCommandOption>.AutoExecute(bool autoExecute)
        {
            AutoExecute = autoExecute;

            return this;
        }

        ICommandConfigurationBuilder ICommandConfigurationBuilder.AutoExecute(bool autoExecute)
        {
            AutoExecute = autoExecute;

            return this;
        }
    }
}
