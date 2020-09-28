using MatthiWare.CommandLine.Abstractions;
using MatthiWare.CommandLine.Abstractions.Command;
using MatthiWare.CommandLine.Abstractions.Models;
using MatthiWare.CommandLine.Abstractions.Parsing;
using MatthiWare.CommandLine.Abstractions.Parsing.Command;
using MatthiWare.CommandLine.Abstractions.Validations;
using MatthiWare.CommandLine.Core.Exceptions;
using MatthiWare.CommandLine.Core.Parsing.Command;
using MatthiWare.CommandLine.Core.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

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
        private readonly IServiceProvider m_serviceProvider;
        private readonly CommandLineParserOptions m_parserOptions;
        private readonly IValidatorsContainer m_validators;
        private readonly ILogger logger;
        private readonly IModelInitializer modelInitializer;
        private readonly IArgumentManager argumentManager;
        private Action m_executor1;
        private Action<TOption> m_executor2;
        private Action<TOption, TCommandOption> m_executor3;

        private Func<CancellationToken, Task> m_executorAsync1;
        private Func<TOption, CancellationToken, Task> m_executorAsync2;
        private Func<TOption, TCommandOption, CancellationToken, Task> m_executorAsync3;

        private readonly string m_helpOptionName;
        private readonly string m_helpOptionNameLong;

        public CommandLineCommand(CommandLineParserOptions parserOptions, IServiceProvider serviceProvider, TOption option, IValidatorsContainer validators, ILogger logger, IModelInitializer modelInitializer, IArgumentManager argumentManager)
        {
            m_parserOptions = parserOptions ?? throw new ArgumentNullException(nameof(parserOptions));
            m_commandOption = new TCommandOption();

            m_validators = validators ?? throw new ArgumentNullException(nameof(validators));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.modelInitializer = modelInitializer ?? throw new ArgumentNullException(nameof(modelInitializer));
            this.argumentManager = argumentManager ?? throw new ArgumentNullException(nameof(argumentManager));
            m_serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            m_baseOption = option ?? throw new ArgumentNullException(nameof(option));

            (m_helpOptionName, m_helpOptionNameLong) = parserOptions.GetConfiguredHelpOption();

            InitialzeModel();
        }

        private void ExecuteInternal()
        {
            m_executor3?.Invoke(m_baseOption, m_commandOption);
            m_executor2?.Invoke(m_baseOption);
            m_executor1?.Invoke();
        }

        public override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            logger.LogDebug("Executing async Command '{Name}'", this.Name);

            await ExecuteInternalAsync(cancellationToken);

            // Also executes the sync stuff.
            ExecuteInternal();
        }

        private async Task ExecuteInternalAsync(CancellationToken cancellationToken)
        {
            // await null-conditional doesn't work see https://github.com/dotnet/csharplang/issues/35

            if (m_executorAsync3 != null)
            {
                await m_executorAsync3(m_baseOption, m_commandOption, cancellationToken);
            }

            if (m_executorAsync2 != null)
            {
                await m_executorAsync2(m_baseOption, cancellationToken);
            }

            if (m_executorAsync1 != null)
            {
                await m_executorAsync1(cancellationToken);
            }
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
                var option = ActivatorUtilities.CreateInstance<CommandLineOption<T>>(m_serviceProvider, m_commandOption, selector);

                logger.LogDebug("Add command option builder for {Expression}", key);

                m_options.Add(key, option);
            }

            return m_options[key] as IOptionBuilder<T>;
        }

        public override async Task<ICommandParserResult> ParseAsync(CancellationToken cancellationToken)
        {
            var result = new CommandParserResult(this);
            var errors = new List<Exception>();

            await ParseCommandsAsync(errors, result, cancellationToken);

            ParseOptions(errors, result);

            await ValidateAsync(m_commandOption, errors, cancellationToken);

            result.MergeResult(errors);

            return result;
        }

        private void ParseOptions(IList<Exception> errors, CommandParserResult result)
        {
            foreach (var optionKeyValue in m_options)
            {
                var option = optionKeyValue.Value;
                
                try
                {
                    ParseOption(option, result);

                    if (result.HelpRequested)
                    {
                        break;
                    }
                }
                catch (OptionParseException e)
                {
                    logger.LogDebug("Unable to parse option value '{Value}'", e.ArgumentModel.Value);

                    errors.Add(e);
                }
                catch (OptionNotFoundException e)
                {
                    logger.LogDebug("Command Option '{Name}' not found! Option is marked as required, with no default values configured.", option.ShortName);

                    errors.Add(e);
                }
                catch (Exception e)
                {
                    logger.LogError(e, "Command Option '{Name}' unknown error occured during parsing.", option.ShortName);

                    errors.Add(e);
                }
            }
        }

        private void ParseOption(CommandLineOptionBase option, CommandParserResult result)
        {
            bool found = argumentManager.TryGetValue(option, out ArgumentModel model);

            if (found && HelpRequested(result, option, model))
            {
                logger.LogDebug("Command Option '{Name}' got help requested.", option.ShortName);

                return;
            }
            else if (!found && option.CheckOptionNotFound())
            {
                throw new OptionNotFoundException(option);
            }
            else if (option.ShouldUseDefault(found, model))
            {
                logger.LogDebug("Command Option '{Name}' using default value.", option.ShortName);

                option.UseDefault();

                return;
            }
            else if (found && !option.CanParse(model))
            {
                throw new OptionParseException(option, model);
            }

            option.Parse(model);
        }

        private async Task ParseCommandsAsync(IList<Exception> errors, CommandParserResult result, CancellationToken cancellationToken)
        {
            foreach (var cmd in m_commands)
            {
                try
                {
                    await ParseCommandAsync(cmd, result, cancellationToken);

                    if (result.HelpRequested)
                    {
                        break;
                    }
                }
                catch (CommandNotFoundException e)
                {
                    logger.LogDebug("Command '{Name}' not found", cmd.Name);

                    errors.Add(e);
                }
                catch (CommandParseException e)
                {
                    logger.LogDebug("Unable to parse command '{Name}'", cmd.Name);

                    errors.Add(e);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Unknown error occured while parsing the commands");

                    errors.Add(ex);
                }
            }
        }

        private async Task ParseCommandAsync(CommandLineCommandBase cmd, CommandParserResult result, CancellationToken cancellationToken)
        {
            if (!argumentManager.TryGetValue(cmd, out _))
            {
                result.MergeResult(new CommandNotFoundParserResult(cmd));

                if (cmd.IsRequired)
                {
                    throw new CommandNotFoundException(cmd);
                }

                return;
            }

            var cmdParseResult = await cmd.ParseAsync(cancellationToken);

            if (cmdParseResult.HelpRequested)
            {
                return;
            }

            result.MergeResult(cmdParseResult);

            if (cmdParseResult.HasErrors)
            {
                throw new CommandParseException(cmd, cmdParseResult.Errors);
            }
        }

        private bool HelpRequested(CommandParserResult result, CommandLineOptionBase option, ArgumentModel model)
        {
            if (!m_parserOptions.EnableHelpOption)
            {
                return false;
            }

            if (IsHelpOption(model.Key))
            {
                result.HelpRequestedFor = this;

                return true;
            }
            else if (model.HasValue && IsHelpOption(model.Value))
            {
                result.HelpRequestedFor = option;

                return true;
            }

            return false;
        }

        private bool IsHelpOption(string input)
            => input.EqualsIgnoreCase(m_helpOptionName) || input.EqualsIgnoreCase(m_helpOptionNameLong);

        private async Task ValidateAsync<T>(T @object, List<Exception> errors, CancellationToken token)
        {
            if (!m_validators.HasValidatorFor<T>())
            {
                logger.LogDebug("No validator configured for {name} in command '{cmdName}'", typeof(T).Name, Name);

                return;
            }

            var results = (await Task.WhenAll(m_validators.GetValidators<T>()
                .Select(async validator => await validator.ValidateAsync(@object, token))))
                .ToArray();

            foreach (var result in results)
            {
                if (result.IsValid)
                {
                    continue;
                }

                logger.LogDebug("Validation failed with '{message}'", result.Error.Message);

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
            m_executor2 = action;

            return this;
        }

        public ICommandBuilder<TOption, TCommandOption> OnExecuting(Action action)
        {
            m_executor1 = action;

            return this;
        }

        public ICommandBuilder<TOption, TCommandOption> OnExecuting(Action<TOption, TCommandOption> action)
        {
            m_executor3 = action;

            return this;
        }

        public ICommandBuilder<TOption, TCommandOption> OnExecutingAsync(Func<CancellationToken, Task> action)
        {
            m_executorAsync1 = action;

            return this;
        }

        public ICommandBuilder<TOption, TCommandOption> OnExecutingAsync(Func<TOption, CancellationToken, Task> action)
        {
            m_executorAsync2 = action;

            return this;
        }

        public ICommandBuilder<TOption, TCommandOption> OnExecutingAsync(Func<TOption, TCommandOption, CancellationToken, Task> action)
        {
            m_executorAsync3 = action;

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
            m_executor2 = action;

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
            modelInitializer.InitializeModel(typeof(TCommandOption), this, nameof(ConfigureInternal), nameof(RegisterCommand));
        }

        /// <summary>
        /// Registers a command type
        /// </summary>
        /// <typeparam name="TCommand">Command type, must be inherit <see cref="Abstractions.Command.Command"/></typeparam>
        public void RegisterCommand<TCommand>()
            where TCommand : Abstractions.Command.Command
        {
            var command = ActivatorUtilities.CreateInstance<CommandLineCommand<TOption, object>>(m_serviceProvider, m_commandOption, m_validators);

            if (typeof(TCommand).IsAssignableToGenericType(typeof(Command<>)))
            {
                RegisterGenericCommandInternal<TCommand>(command);
            }
            else
            {
                RegisterNonGenericCommandInternal<TCommand>(command);
            }

            logger.LogDebug("Command registered '{name}' required: {req}, auto exec: {autoExec}", command.Name, command.IsRequired, command.AutoExecute);

            m_commands.Add(command);
        }

        /// <summary>
        /// Registers a command type
        /// </summary>
        /// <typeparam name="TCommand"></typeparam>
        /// <typeparam name="TActualCommandOption"></typeparam>
        public void RegisterCommand<TCommand, TActualCommandOption>()
           where TCommand : Command<TOption, TActualCommandOption>
           where TActualCommandOption : class, new()
        {
            var cmdConfigurator = ActivatorUtilities.GetServiceOrCreateInstance<TCommand>(m_serviceProvider);

            var command = ActivatorUtilities.CreateInstance<CommandLineCommand<TOption, TActualCommandOption>>(m_serviceProvider, m_baseOption, m_validators);

            cmdConfigurator.OnConfigure(command);

            logger.LogDebug("Command registered '{name}' required: {req}, auto exec: {autoExec}", command.Name, command.IsRequired, command.AutoExecute);

            command.OnExecuting((Action<TOption, TActualCommandOption>)cmdConfigurator.OnExecute);
            command.OnExecutingAsync((Func<TOption, TActualCommandOption, CancellationToken, Task>)cmdConfigurator.OnExecuteAsync);

            m_commands.Add(command);
        }

        private void RegisterGenericCommandInternal<TCommand>(CommandLineCommand<TOption, object> command)
           where TCommand : Abstractions.Command.Command
        {
            var cmdConfigurator = (Command<TOption>)(Abstractions.Command.Command)(ActivatorUtilities.GetServiceOrCreateInstance<TCommand>(m_serviceProvider));

            cmdConfigurator.OnConfigure(command);

            command.OnExecuting((Action<TOption>)cmdConfigurator.OnExecute);
            command.OnExecutingAsync((Func<TOption, CancellationToken, Task>)cmdConfigurator.OnExecuteAsync);
        }

        private void RegisterNonGenericCommandInternal<TCommand>(CommandLineCommand<TOption, object> command)
            where TCommand : Abstractions.Command.Command
        {
            var cmdConfigurator = ActivatorUtilities.GetServiceOrCreateInstance<TCommand>(m_serviceProvider);

            cmdConfigurator.OnConfigure(command);

            command.OnExecuting(cmdConfigurator.OnExecute);
            command.OnExecutingAsync(cmdConfigurator.OnExecuteAsync);
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

        ICommandBuilder<TOption> ICommandBuilder<TOption>.OnExecutingAsync(Func<TOption, CancellationToken, Task> action)
        {
            m_executorAsync2 = action;

            return this;
        }
    }
}
