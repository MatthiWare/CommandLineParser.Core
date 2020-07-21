using MatthiWare.CommandLine.Abstractions;
using MatthiWare.CommandLine.Abstractions.Command;
using MatthiWare.CommandLine.Abstractions.Models;
using MatthiWare.CommandLine.Abstractions.Parsing;
using MatthiWare.CommandLine.Abstractions.Parsing.Command;
using MatthiWare.CommandLine.Abstractions.Usage;
using MatthiWare.CommandLine.Abstractions.Validations;
using MatthiWare.CommandLine.Core;
using MatthiWare.CommandLine.Core.Attributes;
using MatthiWare.CommandLine.Core.Command;
using MatthiWare.CommandLine.Core.Exceptions;
using MatthiWare.CommandLine.Core.Parsing;
using MatthiWare.CommandLine.Core.Parsing.Command;
using MatthiWare.CommandLine.Core.Usage;
using MatthiWare.CommandLine.Core.Utils;
using MatthiWare.CommandLine.Core.Validations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("CommandLineParser.Tests")]

namespace MatthiWare.CommandLine
{
    /// <summary>
    /// Command line parser
    /// </summary>
    /// <typeparam name="TOption">Options model</typeparam>
    public class CommandLineParser<TOption> : ICommandLineParser<TOption>, ICommandLineCommandContainer, IArgument
        where TOption : class, new()
    {
        private readonly TOption m_option;
        private readonly Dictionary<string, CommandLineOptionBase> m_options;
        private readonly List<CommandLineCommandBase> m_commands;
        private readonly string m_helpOptionName;
        private readonly string m_helpOptionNameLong;
        private readonly List<IValidator> validators;
        private readonly ICommandDiscoverer commandDiscoverer = new CommandDiscoverer();

        /// <summary>
        /// <see cref="CommandLineParserOptions"/> this parser is currently using. 
        /// NOTE: In order to use the options they need to be passed using the constructor. 
        /// </summary>
        public CommandLineParserOptions ParserOptions { get; }

        /// <summary>
        /// Tool to print usage info.
        /// </summary>
        public IUsagePrinter Printer { get; set; }

        /// <summary>
        /// Read-only collection of options specified
        /// </summary>
        public IReadOnlyList<ICommandLineOption> Options => new ReadOnlyCollectionWrapper<string, CommandLineOptionBase>(m_options.Values);

        /// <summary>
        /// Factory to create resolvers for options
        /// </summary>
        public IArgumentResolverFactory ArgumentResolverFactory { get; }

        /// <summary>
        /// Resolver that is used to instantiate types by an given container
        /// </summary>
        public IContainerResolver ContainerResolver { get; }

        /// <summary>
        /// Read-only list of commands specified
        /// </summary>
        public IReadOnlyList<ICommandLineCommand> Commands => m_commands.AsReadOnly();

        /// <summary>
        /// Container for all validators
        /// </summary>
        public IValidatorsContainer Validators { get; }

        /// <summary>
        /// Creates a new instance of the commandline parser
        /// </summary>
        public CommandLineParser()
            : this(new CommandLineParserOptions(), new DefaultArgumentResolverFactory(new DefaultContainerResolver()), new DefaultContainerResolver())
        { }

        /// <summary>
        /// Creates a new instance of the commandline parser
        /// </summary>
        /// <param name="parserOptions">The parser options</param>
        public CommandLineParser(CommandLineParserOptions parserOptions)
            : this(parserOptions, new DefaultArgumentResolverFactory(new DefaultContainerResolver()), new DefaultContainerResolver())
        { }

        /// <summary>
        /// Creates a new instance of the commandline parser
        /// </summary>
        /// <param name="argumentResolverFactory">argument resolver to use</param>
        public CommandLineParser(IArgumentResolverFactory argumentResolverFactory)
            : this(new CommandLineParserOptions(), argumentResolverFactory, new DefaultContainerResolver())
        { }

        /// <summary>
        /// Creates a new instance of the commandline parser
        /// </summary>
        /// <param name="parserOptions">options that the parser will use</param>
        /// <param name="argumentResolverFactory">argument resolver to use</param>
        public CommandLineParser(CommandLineParserOptions parserOptions, IArgumentResolverFactory argumentResolverFactory)
            : this(parserOptions, argumentResolverFactory, new DefaultContainerResolver())
        { }

        /// <summary>
        /// Creates a new instance of the commandline parser
        /// </summary>
        /// <param name="containerResolver">container resolver to use</param>
        public CommandLineParser(IContainerResolver containerResolver)
            : this(new CommandLineParserOptions(), new DefaultArgumentResolverFactory(containerResolver), containerResolver)
        { }

        /// <summary>
        /// Creates a new instance of the commandline parser
        /// </summary>
        /// <param name="parserOptions">options that the parser will use</param>
        /// <param name="containerResolver">container resolver to use</param>
        public CommandLineParser(CommandLineParserOptions parserOptions, IContainerResolver containerResolver)
            : this(parserOptions, new DefaultArgumentResolverFactory(containerResolver), containerResolver)
        { }

        /// <summary>
        /// Creates a new instance of the commandline parser
        /// </summary>
        /// <param name="argumentResolverFactory">argument resolver to use</param>
        /// <param name="containerResolver">container resolver to use</param>
        /// <param name="parserOptions">The options the parser will use</param>
        public CommandLineParser(CommandLineParserOptions parserOptions, IArgumentResolverFactory argumentResolverFactory, IContainerResolver containerResolver)
        {
            Validators = new ValidatorsContainer(containerResolver);

            ParserOptions = parserOptions;
            m_option = new TOption();

            m_options = new Dictionary<string, CommandLineOptionBase>();
            m_commands = new List<CommandLineCommandBase>();

            ArgumentResolverFactory = argumentResolverFactory;
            ContainerResolver = containerResolver;

            if (string.IsNullOrWhiteSpace(ParserOptions.AppName))
                ParserOptions.AppName = Process.GetCurrentProcess().ProcessName;

            Printer = new UsagePrinter(this, new UsageBuilder(parserOptions));

            if (ParserOptions.EnableHelpOption)
            {
                var tokens = ParserOptions.HelpOptionName.Split('|');

                if (tokens.Length > 1)
                {
                    m_helpOptionName = $"{ParserOptions.PrefixShortOption}{tokens[0]}";
                    m_helpOptionNameLong = $"{ParserOptions.PrefixLongOption}{tokens[1]}";
                }
                else
                {
                    m_helpOptionName = $"{ParserOptions.PrefixLongOption}{tokens[0]}";
                    m_helpOptionNameLong = null;
                }
            }

            InitialzeModel();
        }

        /// <summary>
        /// Configures an option in the model
        /// </summary>
        /// <typeparam name="TProperty">Type of the property</typeparam>
        /// <param name="selector">Model property to configure</param>
        /// <returns><see cref="IOptionBuilder"/></returns>
        public IOptionBuilder<TProperty> Configure<TProperty>(Expression<Func<TOption, TProperty>> selector)
        {
            var memberInfo = ((MemberExpression)selector.Body).Member;
            var key = $"{memberInfo.DeclaringType.FullName}.{memberInfo.Name}";

            return ConfigureInternal<TProperty>(selector, key);
        }

        private IOptionBuilder<T> ConfigureInternal<T>(LambdaExpression selector, string key)
        {
            if (!m_options.ContainsKey(key))
            {
                var option = new CommandLineOption<T>(ParserOptions, m_option, selector, ArgumentResolverFactory);

                m_options.Add(key, option);
            }

            return m_options[key] as IOptionBuilder<T>;
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

            var argumentManager = new ArgumentManager(args, ParserOptions, m_helpOptionName, m_helpOptionNameLong, m_commands, m_options.Values.Cast<ICommandLineOption>().ToArray());

            ParseCommands(errors, result, argumentManager);

            ParseOptions(errors, result, argumentManager);

            CheckForExtraHelpArguments(result, argumentManager);

            Validate(m_option, errors);

            result.MergeResult(errors);

            AutoExecuteCommands(result);

            AutoPrintUsageAndErrors(result, args.Length == 0);

            return result;
        }

        /// <summary>
        /// Parses the commandline arguments async
        /// </summary>
        /// <param name="args">arguments from the commandline</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The result of the parsing, <see cref="IParserResult{TResult}"/></returns>
        public async Task<IParserResult<TOption>> ParseAsync(string[] args, CancellationToken cancellationToken = default)
        {
            var errors = new List<Exception>();

            var result = new ParseResult<TOption>();

            var argumentManager = new ArgumentManager(args, ParserOptions, m_helpOptionName, m_helpOptionNameLong, m_commands, m_options.Values.Cast<ICommandLineOption>().ToArray());

            await ParseCommandsAsync(errors, result, argumentManager, cancellationToken);

            ParseOptions(errors, result, argumentManager);

            CheckForExtraHelpArguments(result, argumentManager);

            await ValidateAsync(m_option, errors, cancellationToken);

            result.MergeResult(errors);

            await AutoExecuteCommandsAsync(result, cancellationToken);

            AutoPrintUsageAndErrors(result, args.Length == 0);

            return result;
        }

        private void Validate<T>(T @object, List<Exception> errors)
        {
            if (!Validators.HasValidatorFor<T>())
                return;

            var results = Validators.GetValidators<T>().Select(validator => validator.Validate(@object)).ToArray();

            foreach (var result in results)
            {
                if (result.IsValid)
                    continue;

                errors.Add(result.Error);
            }
        }

        private async Task ValidateAsync<T>(T @object, List<Exception> errors, CancellationToken token)
        {
            if (!Validators.HasValidatorFor<T>())
                return;

            var results = (await Task.WhenAll(Validators.GetValidators<T>()
                .Select(async validator => await validator.ValidateAsync(@object, token)))).ToArray();

            foreach (var result in results)
            {
                if (result.IsValid)
                    continue;

                errors.Add(result.Error);
            }
        }

        private void CheckForExtraHelpArguments(ParseResult<TOption> result, ArgumentManager argumentManager)
        {
            var unusedArg = argumentManager.UnusedArguments
                .Where(a => string.Equals(a.Argument, m_helpOptionName, StringComparison.InvariantCultureIgnoreCase) ||
                string.Equals(a.Argument, m_helpOptionNameLong, StringComparison.InvariantCultureIgnoreCase))
                .FirstOrDefault();

            if (unusedArg == null) return;

            result.HelpRequestedFor = unusedArg.ArgModel ?? this;
        }

        private void AutoPrintUsageAndErrors(ParseResult<TOption> result, bool noArgsSupplied)
        {
            if (!ParserOptions.AutoPrintUsageAndErrors) return;

            if (noArgsSupplied && (Options.Any(opt => !opt.HasDefault) || Commands.Any(cmd => cmd.IsRequired)))
                PrintHelp();
            else if (result.HelpRequested)
                PrintHelpRequestedForArgument(result.HelpRequestedFor);
            else if (result.HasErrors)
                PrintErrors(result.Errors);
        }

        private void PrintHelpRequestedForArgument(IArgument argument)
        {
            switch (argument)
            {
                case ICommandLineCommand cmd:
                    Printer.PrintCommandUsage(cmd);
                    break;
                case ICommandLineOption opt:
                    Printer.PrintOptionUsage(opt);
                    break;
                default:
                    PrintHelp();
                    break;
            }
        }

        private void PrintErrors(IReadOnlyCollection<Exception> errors)
        { 
            Printer.PrintErrors(errors);
            Printer.PrintUsage();
        }

        private void PrintHelp() => Printer.PrintUsage();

        private void AutoExecuteCommands(ParseResult<TOption> result)
        {
            if (result.HasErrors) return;

            ExecuteCommandParserResults(result, result.CommandResults.Where(sub => sub.Command.AutoExecute));
        }

        private async Task AutoExecuteCommandsAsync(ParseResult<TOption> result, CancellationToken cancellationToken)
        {
            if (result.HasErrors) return;

            await ExecuteCommandParserResultsAsync(result, result.CommandResults.Where(sub => sub.Command.AutoExecute), cancellationToken);
        }

        private bool HelpRequested(ParseResult<TOption> result, CommandLineOptionBase option, ArgumentModel model)
        {
            if (!ParserOptions.EnableHelpOption) return false;

            if (model.HasValue &&
              (model.Value.Equals(m_helpOptionName, StringComparison.InvariantCultureIgnoreCase) ||
              model.Value.Equals(m_helpOptionNameLong, StringComparison.InvariantCultureIgnoreCase)))
            {
                result.HelpRequestedFor = option;

                return true;
            }

            return false;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Commands can throw all sorts of exceptions when executing")]
        private void ExecuteCommandParserResults(ParseResult<TOption> results, IEnumerable<ICommandParserResult> cmds)
        {
            var errors = new List<Exception>();

            foreach (var cmd in cmds)
            {
                try
                {
                    cmd.ExecuteCommand();
                }
                catch (Exception ex)
                {
                    errors.Add(new CommandExecutionFailedException(cmd.Command, ex));
                }
            }

            if (errors.Any())
                results.MergeResult(errors);

            foreach (var cmd in cmds)
                ExecuteCommandParserResults(results, cmd.SubCommands.Where(sub => sub.Command.AutoExecute));
        }

        private async Task ExecuteCommandParserResultsAsync(ParseResult<TOption> results, IEnumerable<ICommandParserResult> cmds, CancellationToken cancellationToken)
        {
            var errors = new List<Exception>();

            foreach (var cmd in cmds)
            {
                try
                {
                    await cmd.ExecuteCommandAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    errors.Add(new CommandExecutionFailedException(cmd.Command, ex));
                }
            }

            if (errors.Any())
                results.MergeResult(errors);

            foreach (var cmd in cmds)
                await ExecuteCommandParserResultsAsync(results, cmd.SubCommands.Where(sub => sub.Command.AutoExecute), cancellationToken);
        }

        private void ParseCommands(IList<Exception> errors, ParseResult<TOption> result, IArgumentManager argumentManager)
        {
            foreach (var cmd in m_commands)
            {
                try
                {
                    ParseCommand(cmd, result, argumentManager);

                    if (result.HelpRequested)
                        break;
                }
                catch (Exception ex)
                {
                    errors.Add(ex);
                }
            }
        }

        private void ParseCommand(CommandLineCommandBase cmd, ParseResult<TOption> result, IArgumentManager argumentManager)
        {
            if (!argumentManager.TryGetValue(cmd, out _))
            {
                result.MergeResult(new CommandNotFoundParserResult(cmd));

                if (cmd.IsRequired)
                    throw new CommandNotFoundException(cmd);

                return;
            }

            var cmdParseResult = cmd.Parse(argumentManager);

            result.MergeResult(cmdParseResult);

            if (cmdParseResult.HasErrors)
                throw new CommandParseException(cmd, cmdParseResult.Errors);
        }

        private async Task ParseCommandsAsync(IList<Exception> errors, ParseResult<TOption> result, IArgumentManager argumentManager, CancellationToken cancellationToken)
        {
            foreach (var cmd in m_commands)
            {
                try
                {
                    await ParseCommandAsync(cmd, result, argumentManager, cancellationToken);

                    if (result.HelpRequested)
                        break;
                }
                catch (Exception ex)
                {
                    errors.Add(ex);
                }
            }
        }

        private async Task ParseCommandAsync(CommandLineCommandBase cmd, ParseResult<TOption> result, IArgumentManager argumentManager, CancellationToken cancellationToken)
        {
            if (!argumentManager.TryGetValue(cmd, out _))
            {
                result.MergeResult(new CommandNotFoundParserResult(cmd));

                if (cmd.IsRequired)
                    throw new CommandNotFoundException(cmd);

                return;
            }

            var cmdParseResult = await cmd.ParseAsync(argumentManager, cancellationToken);

            result.MergeResult(cmdParseResult);

            if (cmdParseResult.HasErrors)
                throw new CommandParseException(cmd, cmdParseResult.Errors);
        }

        private void ParseOptions(IList<Exception> errors, ParseResult<TOption> result, IArgumentManager argumentManager)
        {
            foreach (var o in m_options)
            {
                try
                {
                    if (ParseOption(o.Value, result, argumentManager))
                        break; // break here because help is requested!
                }
                catch (Exception ex)
                {
                    errors.Add(ex);
                }
            }

            result.MergeResult(m_option);
        }

        private bool ParseOption(CommandLineOptionBase option, ParseResult<TOption> result, IArgumentManager argumentManager)
        {
            bool found = argumentManager.TryGetValue(option, out ArgumentModel model);

            if (found && HelpRequested(result, option, model))
            {
                return true;
            }
            else if (!found && option.IsRequired && !option.HasDefault)
            {
                throw new OptionNotFoundException(option);
            }
            else if ((!found && !model.HasValue && option.HasDefault) ||
                (found && !option.CanParse(model) && option.HasDefault))
            {
                option.UseDefault();

                return false;
            }
            else if (found && !option.CanParse(model))
            {
                throw new OptionParseException(option, model);
            }

            option.Parse(model);

            return false;
        }

        /// <summary>
        /// Adds a command to the parser
        /// </summary>
        /// <typeparam name="TCommandOption">Options model for the command</typeparam>
        /// <returns>Builder for the command, <see cref="ICommandBuilder{TOption,TCommandOption}"/></returns>
        public ICommandBuilder<TOption, TCommandOption> AddCommand<TCommandOption>() where TCommandOption : class, new()
        {
            var command = new CommandLineCommand<TOption, TCommandOption>(ParserOptions, ArgumentResolverFactory, ContainerResolver, m_option, Validators);

            m_commands.Add(command);

            return command;
        }

        /// <summary>
        /// Registers a command type
        /// </summary>
        /// <typeparam name="TCommand">Command type, must be inherit <see cref="Command{TOptions}"/></typeparam>
        public void RegisterCommand<TCommand>()
            where TCommand : Command<TOption>
        {
            var cmdConfigurator = ContainerResolver.Resolve<TCommand>();

            var command = new CommandLineCommand<TOption, object>(ParserOptions, ArgumentResolverFactory, ContainerResolver, m_option, Validators);

            cmdConfigurator.OnConfigure(command);

            command.OnExecuting((Action<TOption>)cmdConfigurator.OnExecute);
            command.OnExecutingAsync((Func<TOption, CancellationToken, Task>)cmdConfigurator.OnExecuteAsync);

            m_commands.Add(command);
        }

        /// <summary>
        /// Registers a new command
        /// </summary>
        /// <param name="commandType">The type of the command</param>
        public void RegisterCommand(Type commandType) => RegisterCommand(commandType, null);

        /// <summary>
        /// Registers a command type
        /// </summary>
        /// <typeparam name="TCommand">Command type, must be inherit <see cref="Command{TOptions,TCommandOption}"/></typeparam>
        /// <typeparam name="TCommandOption">The command options</typeparam>
        public void RegisterCommand<TCommand, TCommandOption>()
           where TCommand : Command<TOption, TCommandOption>
           where TCommandOption : class, new()
        {
            var cmdConfigurator = ContainerResolver.Resolve<TCommand>();

            var command = new CommandLineCommand<TOption, TCommandOption>(ParserOptions, ArgumentResolverFactory, ContainerResolver, m_option, Validators);

            cmdConfigurator.OnConfigure((ICommandConfigurationBuilder<TCommandOption>)command);

            command.OnExecuting((Action<TOption, TCommandOption>)cmdConfigurator.OnExecute);
            command.OnExecutingAsync((Func<TOption, TCommandOption, CancellationToken, Task>)cmdConfigurator.OnExecuteAsync);

            m_commands.Add(command);
        }

        /// <summary>
        /// Registers a new command
        /// </summary>
        /// <param name="commandType">The type of the command</param>
        /// <param name="optionsType">Command options model</param>
        public void RegisterCommand(Type commandType, Type optionsType)
        {
            if (!commandType.IsAssignableToGenericType(typeof(Command<>)))
            {
                throw new ArgumentException($"Provided command {commandType} is not assignable to {typeof(Command<>)}");
            }

            this.ExecuteGenericRegisterCommand(nameof(RegisterCommand), commandType, optionsType);
        }

        /// <summary>
        /// Adds a command to the parser
        /// </summary>
        /// <returns>Builder for the command, <see cref="ICommandBuilder{TOption}"/></returns>
        public ICommandBuilder<TOption> AddCommand()
        {
            var command = new CommandLineCommand<TOption, object>(ParserOptions, ArgumentResolverFactory, ContainerResolver, m_option, Validators);

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

                var cmdType = propInfo.PropertyType;

                if (cmdType.IsAssignableToGenericType(typeof(Command<>)))
                {
                    var genericTypes = cmdType.BaseType.GenericTypeArguments;

                    this.ExecuteGenericRegisterCommand(nameof(RegisterCommand), cmdType);
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
        /// Discovers commands and registers them from any given assembly
        /// </summary>
        /// <param name="assembly">Assembly containing the command types</param>
        public void DiscoverCommands(Assembly assembly) => DiscoverCommands(new[] { assembly });

        /// <summary>
        /// Discovers commands and registers them from any given assembly
        /// </summary>
        /// <param name="assemblies">Assemblies containing the command types</param>
        public void DiscoverCommands(Assembly[] assemblies)
        {
            var commandTypes = commandDiscoverer.DiscoverCommandTypes(typeof(TOption), assemblies);

            foreach (var commandType in commandTypes)
            {
                this.ExecuteGenericRegisterCommand(nameof(RegisterCommand), commandType);
            }
        }
    }
}
