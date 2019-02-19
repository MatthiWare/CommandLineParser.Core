using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

using MatthiWare.CommandLine.Abstractions;
using MatthiWare.CommandLine.Abstractions.Command;
using MatthiWare.CommandLine.Abstractions.Models;
using MatthiWare.CommandLine.Abstractions.Parsing;
using MatthiWare.CommandLine.Abstractions.Parsing.Command;
using MatthiWare.CommandLine.Abstractions.Usage;
using MatthiWare.CommandLine.Core;
using MatthiWare.CommandLine.Core.Attributes;
using MatthiWare.CommandLine.Core.Command;
using MatthiWare.CommandLine.Core.Exceptions;
using MatthiWare.CommandLine.Core.Parsing;
using MatthiWare.CommandLine.Core.Parsing.Command;
using MatthiWare.CommandLine.Core.Usage;
using MatthiWare.CommandLine.Core.Utils;

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
        /// Creates a new instance of the commandline parser
        /// </summary>
        public CommandLineParser()
            : this(new CommandLineParserOptions(), new DefaultArgumentResolverFactory(), new DefaultContainerResolver())
        { }

        /// <summary>
        /// Creates a new instance of the commandline parser
        /// </summary>
        /// <param name="parserOptions">The parser options</param>
        public CommandLineParser(CommandLineParserOptions parserOptions)
            : this(parserOptions, new DefaultArgumentResolverFactory(), new DefaultContainerResolver())
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
            : this(new CommandLineParserOptions(), new DefaultArgumentResolverFactory(), containerResolver)
        { }

        /// <summary>
        /// Creates a new instance of the commandline parser
        /// </summary>
        /// <param name="parserOptions">options that the parser will use</param>
        /// <param name="containerResolver">container resolver to use</param>
        public CommandLineParser(CommandLineParserOptions parserOptions, IContainerResolver containerResolver)
            : this(parserOptions, new DefaultArgumentResolverFactory(), containerResolver)
        { }

        /// <summary>
        /// Creates a new instance of the commandline parser
        /// </summary>
        /// <param name="argumentResolverFactory">argument resolver to use</param>
        /// <param name="containerResolver">container resolver to use</param>
        public CommandLineParser(CommandLineParserOptions parserOptions, IArgumentResolverFactory argumentResolverFactory, IContainerResolver containerResolver)
        {
            ParserOptions = parserOptions;
            m_option = new TOption();

            m_options = new Dictionary<string, CommandLineOptionBase>();
            m_commands = new List<CommandLineCommandBase>();

            ArgumentResolverFactory = argumentResolverFactory;
            ContainerResolver = containerResolver;

            if (string.IsNullOrWhiteSpace(ParserOptions.AppName))
                ParserOptions.AppName = Process.GetCurrentProcess().ProcessName;

            Printer = new UsagePrinter(ParserOptions, this);

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
                var option = new CommandLineOption(ParserOptions, m_option, selector, ArgumentResolverFactory);

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

            var argumentManager = new ArgumentManager(args, ParserOptions.EnableHelpOption, m_helpOptionName, m_helpOptionNameLong, m_commands, m_options.Values);

            ParseCommands(errors, result, argumentManager);

            ParseOptions(errors, result, argumentManager);

            CheckForExtraHelpArguments(result, argumentManager);

            result.MergeResult(errors);

            AutoExecuteCommands(result);

            AutoPrintUsageAndErrors(result, args.Length == 0);

            return result;
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

            if (noArgsSupplied)
                PrintHelp();
            else if (result.HelpRequested)
                PrintHelpFor(result.HelpRequestedFor);
            else if (result.HasErrors)
                PrintErrors(result.Errors);
        }

        private void PrintHelpFor(IArgument helpRequestedFor)
        {
            switch (helpRequestedFor)
            {
                case ICommandLineCommand cmd:
                    Printer.PrintUsage(cmd);
                    break;
                case ICommandLineOption opt:
                    Printer.PrintUsage(opt);
                    break;
                default:
                    PrintHelp();
                    break;
            }
        }

        private void PrintErrors(IReadOnlyCollection<Exception> errors)
        {
            var previousColor = Console.ForegroundColor;

            Console.ForegroundColor = ConsoleColor.DarkRed;

            foreach (var error in errors)
                Console.Error.WriteLine(error.Message);

            Console.ForegroundColor = previousColor;

            PrintHelp();
        }

        private void PrintHelp() => Printer.PrintUsage();

        private void AutoExecuteCommands(ParseResult<TOption> result)
        {
            if (result.HasErrors) return;

            ExecuteCommandParserResults(result, result.CommandResults.Where(sub => sub.Command.AutoExecute));
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
                    errors.Add(ex);
                }
            }

            if (errors.Any())
                results.MergeResult(errors);

            foreach (var cmd in cmds)
                ExecuteCommandParserResults(results, cmd.SubCommands.Where(sub => sub.Command.AutoExecute));
        }

        private void ParseCommands(IList<Exception> errors, ParseResult<TOption> result, IArgumentManager argumentManager)
        {
            foreach (var cmd in m_commands)
            {
                if (!argumentManager.TryGetValue(cmd, out _))
                {
                    if (cmd.IsRequired)
                        errors.Add(new CommandNotFoundException(cmd));

                    result.MergeResult(new CommandNotFoundParserResult(cmd));

                    continue;
                }

                var cmdParseResult = cmd.Parse(argumentManager);

                if (result.HelpRequested)
                    break;

                if (cmdParseResult.HasErrors)
                    errors.Add(new CommandParseException(cmd, cmdParseResult.Errors));

                result.MergeResult(cmdParseResult);
            }
        }

        private void ParseOptions(IList<Exception> errors, ParseResult<TOption> result, IArgumentManager argumentManager)
        {
            foreach (var o in m_options)
            {
                var option = o.Value;
                bool found = argumentManager.TryGetValue(option, out ArgumentModel model);

                if (found && HelpRequested(result, option, model))
                {
                    break;
                }
                else if (!found && option.IsRequired && !option.HasDefault)
                {
                    errors.Add(new OptionNotFoundException(option));

                    continue;
                }
                else if ((!found && !model.HasValue && option.HasDefault) ||
                    (found && !option.CanParse(model) && option.HasDefault))
                {
                    option.UseDefault();

                    continue;
                }
                else if (found && !option.CanParse(model))
                {
                    errors.Add(new OptionParseException(option, model));

                    continue;
                }

                option.Parse(model);
            }

            result.MergeResult(m_option);
        }

        /// <summary>
        /// Adds a command to the parser
        /// </summary>
        /// <typeparam name="TCommandOption">Options model for the command</typeparam>
        /// <returns>Builder for the command, <see cref="ICommandBuilder{TOption,TCommandOption}"/></returns>
        public ICommandBuilder<TOption, TCommandOption> AddCommand<TCommandOption>() where TCommandOption : class, new()
        {
            var command = new CommandLineCommand<TOption, TCommandOption>(ParserOptions, ArgumentResolverFactory, ContainerResolver, m_option);

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

            var command = new CommandLineCommand<TOption, object>(ParserOptions, ArgumentResolverFactory, ContainerResolver, m_option);

            cmdConfigurator.OnConfigure(command);

            command.OnExecuting((Action<TOption>)cmdConfigurator.OnExecute);

            m_commands.Add(command);
        }

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

            var command = new CommandLineCommand<TOption, TCommandOption>(ParserOptions, ArgumentResolverFactory, ContainerResolver, m_option);

            cmdConfigurator.OnConfigure((ICommandConfigurationBuilder<TCommandOption>)command);

            command.OnExecuting((Action<TOption, TCommandOption>)cmdConfigurator.OnExecute);

            m_commands.Add(command);
        }

        /// <summary>
        /// Adds a command to the parser
        /// </summary>
        /// <returns>Builder for the command, <see cref="ICommandBuilder{TOption}"/></returns>
        public ICommandBuilder<TOption> AddCommand()
        {
            var command = new CommandLineCommand<TOption, object>(ParserOptions, ArgumentResolverFactory, ContainerResolver, m_option);

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
                        case DescriptionAttribute helpText:
                            actions.Add(() => ConfigureInternal(lambda, key).Description(helpText.Description));
                            break;
                        case NameAttribute name:
                            actions.Add(() => ConfigureInternal(lambda, key).Name(name.ShortName, name.LongName));
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
        }
    }
}
