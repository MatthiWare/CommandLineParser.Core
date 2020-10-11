using MatthiWare.CommandLine.Abstractions;
using MatthiWare.CommandLine.Abstractions.Command;
using MatthiWare.CommandLine.Abstractions.Models;
using MatthiWare.CommandLine.Abstractions.Parsing;
using MatthiWare.CommandLine.Core.Utils;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MatthiWare.CommandLine.Core.Parsing
{
    /// <inheritdoc/>
    public class ArgumentManager : IArgumentManager
    {
        private readonly CommandLineParserOptions options;
        private readonly ICommandLineCommandContainer commandContainer;
        private readonly ILogger logger;
        private IEnumerator<ArgumentRecord> enumerator;
        private readonly Dictionary<IArgument, ArgumentModel> results = new Dictionary<IArgument, ArgumentModel>();
        private readonly List<UnusedArgumentModel> unusedArguments = new List<UnusedArgumentModel>();
        private ProcessingContext CurrentContext { get; set; }
        private bool isFirstArgument = true;

        /// <inheritdoc/>
        public IReadOnlyList<UnusedArgumentModel> UnusedArguments => unusedArguments;

        /// <inheritdoc/>
        public bool TryGetValue(IArgument argument, out ArgumentModel model) => results.TryGetValue(argument, out model);

        /// <inheritdoc/>
        public ArgumentManager(CommandLineParserOptions options, ICommandLineCommandContainer commandContainer, ILogger<CommandLineParser> logger)
        {
            this.options = options ?? throw new ArgumentNullException(nameof(options));
            this.commandContainer = commandContainer ?? throw new ArgumentNullException(nameof(commandContainer));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc/>
        public void Process(IReadOnlyList<string> arguments, IList<Exception> errors)
        {
            results.Clear();
            unusedArguments.Clear();

            enumerator = new ArgumentRecordEnumerator(options, arguments);
            CurrentContext = new ProcessingContext(null, commandContainer, logger);

            isFirstArgument = true;

            try
            {
                while (enumerator.MoveNext())
                {
                    var processed = ProcessNext();

                    if (!processed)
                    {
                        AddUnprocessedArgument(enumerator.Current);
                    }

                    isFirstArgument = false;
                }
            }
            catch (InvalidOperationException invalidOpException)
            {
                logger.LogError(invalidOpException, "Error occured while processing the argument list");
                errors.Add(invalidOpException);
            }
            finally
            {
                enumerator?.Dispose();
                CurrentContext = null;
            }
        }

        private bool ProcessNext()
        {
            switch (enumerator.Current)
            {
                case OptionRecord option:
                    return ProcessOption(option);
                case CommandOrOptionValueRecord commandOrValue:
                    return ProcessCommandOrOptionValue(commandOrValue);
                case StopProcessingRecord _:
                    return StopProcessing();
                default:
                    return false;
            }
        }

        private bool StopProcessing()
        {
            while (enumerator.MoveNext())
            { 
                // do nothing
            }

            return true;
        }

        private void AddUnprocessedArgument(ArgumentRecord rec)
        {
            //if (isFirstArgument)
            //{
            //    return;
            //}

            var arg = CurrentContext.CurrentOption != null ? (IArgument)CurrentContext.CurrentOption : (IArgument)CurrentContext.CurrentCommand;
            var item = new UnusedArgumentModel(rec.RawData, arg);

            unusedArguments.Add(item);
        }

        private bool ProcessOption(OptionRecord rec)
        {
            var foundOption = FindOption(rec);

            if (!foundOption)
            {
                // In case we have an option named "-1" and int value -1. This causes confusion. 
                return ProcessCommandOrOptionValue(rec);
            }

            return true;
        }

        private bool FindOption(OptionRecord rec)
        {
            var context = CurrentContext;

            while (context != null)
            {
                foreach (var option in context.CurrentCommand.Options)
                {
                    if (!rec.Name.EqualsIgnoreCase(rec.IsLongOption ? option.LongName : option.ShortName))
                    {
                        continue;
                    }

                    if (results.ContainsKey(option))
                    {
                        if (option.AllowMultipleValues)
                        {
                            context.AssertSafeToSwitchProcessingContext();
                            context.CurrentOption = option;

                            return true;
                        }

                        continue;
                    }

                    context.AssertSafeToSwitchProcessingContext();

                    context.CurrentOption = option;

                    var argumentModel = new ArgumentModel(rec.Name, rec.Value);

                    results.Add(option, argumentModel);

                    return true;
                }

                if (ProcessClusteredOptions(context, rec))
                {
                    return true;
                }

                context = context.Parent;
            }

            return false;
        }

        private bool ProcessClusteredOptions(ProcessingContext context, OptionRecord rec)
        {
            var tokens = rec.Name.WithoutPreAndPostfixes(options);

            var list = new List<ICommandLineOption>();

            foreach (var token in tokens)
            {
                bool found = false;

                string key = $"{options.PrefixShortOption}{token}";

                foreach (var option in context.CurrentCommand.Options.Where(ValidClusteredOption))
                {
                    if (!option.ShortName.EqualsIgnoreCase(key))
                    {
                        continue;
                    }

                    var model = new ArgumentModel(key);

                    list.Add(option);
                    results.Add(option, model);

                    found = true;
                    break;
                }

                if (!found)
                {
                    return false;
                }
            }

            context.NextArgumentIsForClusteredOptions = true;
            context.ProcessedClusteredOptions = list;

            return true;
        }

        private bool ValidClusteredOption(ICommandLineOption option)
        {
            if (!option.HasShortName)
            {
                return false;
            }

            if (results.ContainsKey(option))
            {
                return false;
            }

            return true;
        }

        private bool ProcessCommandOrOptionValue(ArgumentRecord rec)
        {
            foreach (var cmd in CurrentContext.CurrentCommand.Commands)
            {
                if (!rec.RawData.EqualsIgnoreCase(cmd.Name))
                {
                    continue;
                }

                results.Add(cmd, new ArgumentModel(cmd.Name, null));

                CurrentContext = new ProcessingContext(CurrentContext, (ICommandLineCommandContainer)cmd, logger);

                return true;
            }

            var context = CurrentContext;

            while (context != null)
            {
                if (context.NextArgumentIsForClusteredOptions)
                {
                    foreach (var option in context.ProcessedClusteredOptions)
                    {
                        results[option].Values.Add(rec.RawData);
                    }

                    context.ProcessedClusteredOptions = null;
                    context.NextArgumentIsForClusteredOptions = false;
                }

                if (context.CurrentOption == null)
                {
                    context = context.Parent;
                    continue;
                }

                if (!TryGetValue(context.CurrentOption, out var model))
                {
                    // not sure yet what to do here.. 
                    // no option yet and not matching command => unknown item
                    context = context.Parent;
                    continue;
                }

                if (model.HasValue && !context.CurrentOption.AllowMultipleValues)
                {
                    // multiple values are not allowed for this option type
                    context = context.Parent;
                    continue;
                }

                model.Values.Add(rec.RawData);
                return true;
            }

            context = CurrentContext;

            //if (isFirstArgument)
            //{
            //    return false;
            //}

            while (context != null)
            {
                foreach (var option in context.CurrentCommand.Options.Where(opt => opt.Order.HasValue).OrderBy(opt => opt.Order))
                {
                    if (results.ContainsKey(option))
                    {
                        continue;
                    }

                    var argumentModel = new ArgumentModel(string.Empty, rec.RawData);

                    results.Add(option, argumentModel);

                    context.CurrentOption = option;
                    context.MarkOrderedOptionAsProcessed(option);

                    return true;
                }

                context = context.Parent;
            }

            return false;
        }

        private class ProcessingContext
        {
            private readonly List<ICommandLineOption> orderedOptions;
            private readonly ILogger logger;

            public ICommandLineOption CurrentOption { get; set; }
            public ProcessingContext Parent { get; set; }
            public ICommandLineCommandContainer CurrentCommand { get; set; }
            public bool HasOrderedOptions { get; }
            public bool AllOrderedOptionsProcessed => orderedOptions.Count == 0;
            public bool ProcessingOrderedOptions { get; private set; }
            public bool NextArgumentIsForClusteredOptions { get; set; }
            public List<ICommandLineOption> ProcessedClusteredOptions { get; set; }

            public ProcessingContext(ProcessingContext parent, ICommandLineCommandContainer commandContainer, ILogger logger)
            {
                this.logger = logger;

                parent?.AssertSafeToSwitchProcessingContext();

                ProcessingOrderedOptions = false;
                Parent = parent;
                CurrentCommand = commandContainer;
                orderedOptions = new List<ICommandLineOption>(CurrentCommand.Options.Where(o => o.Order.HasValue));
                HasOrderedOptions = orderedOptions.Count > 0;
            }

            public void MarkOrderedOptionAsProcessed(ICommandLineOption option)
            {
                ProcessingOrderedOptions = true;
                orderedOptions.Remove(option);

                if (AllOrderedOptionsProcessed)
                {
                    ProcessingOrderedOptions = false;
                }
            }

            public void AssertSafeToSwitchProcessingContext()
            {
                if (!ProcessingOrderedOptions || AllOrderedOptionsProcessed)
                {
                    ProcessedClusteredOptions = null;
                    NextArgumentIsForClusteredOptions = false;
                    return;
                }

                logger.LogDebug("Not all ordered options where processed before switching to a new context. The current context is '{ctx}' with {amount} of options left unprocessed", CurrentCommand.ToString(), orderedOptions.Count);

                foreach (var unprocessedOption in orderedOptions)
                {
                    logger.LogDebug("{Option} was unprocessed", unprocessedOption);
                }

                throw new InvalidOperationException("Not all ordered options where processed before switching to another option/command context!");
            }
        }

        private abstract class ArgumentRecord
        {
            protected ArgumentRecord(string data)
            {
                RawData = data;
            }

            public string RawData { get; }
        }

        private sealed class ArgumentRecordEnumerator : IEnumerator<ArgumentRecord>
        {
            private readonly IEnumerator<string> enumerator;
            private readonly CommandLineParserOptions options;

            public ArgumentRecordEnumerator(CommandLineParserOptions options, IReadOnlyList<string> arguments)
            {
                if (arguments is null)
                {
                    throw new ArgumentNullException(nameof(arguments));
                }

                enumerator = arguments.GetEnumerator();
                this.options = options ?? throw new ArgumentNullException(nameof(options));
            }

            public ArgumentRecord Current { get; private set; }

            object IEnumerator.Current => Current;

            public bool MoveNext()
            {
                if (!enumerator.MoveNext())
                {
                    return false;
                }

                Current = CreateRecord(enumerator.Current);

                return true;
            }

            private ArgumentRecord CreateRecord(string current)
            {
                bool isLongOption = current.StartsWith(options.PrefixLongOption);
                bool isShortOption = current.StartsWith(options.PrefixShortOption);
                bool stopProcessing = !string.IsNullOrEmpty(options.StopParsingAfter) && current.Equals(options.StopParsingAfter);

                if (stopProcessing)
                {
                    return new StopProcessingRecord(current);
                }

                if (isLongOption || isShortOption)
                {
                    return new OptionRecord(current, options.PostfixOption, isLongOption);
                }

                return new CommandOrOptionValueRecord(current);
            }

            public void Reset()
            {
                Current = null;
                enumerator.Reset();
            }

            public void Dispose()
            {
                Current = null;
                enumerator.Dispose();
            }
        }

        private sealed class StopProcessingRecord : ArgumentRecord
        {
            public StopProcessingRecord(string data)
                : base(data)
            {
            }
        }

        private sealed class CommandOrOptionValueRecord : ArgumentRecord
        {
            public CommandOrOptionValueRecord(string data)
                : base(data)
            {
            }
        }

        private sealed class OptionRecord : ArgumentRecord
        {
            public OptionRecord(string data, string postfix, bool isLongOption)
                : base(data)
            {
                var tokens = data.Split(postfix.ToCharArray()[0]);

                if (tokens.Length > 1)
                {
                    Value = tokens[1];
                }

                IsLongOption = isLongOption;
                Name = tokens[0];
            }

            public bool IsLongOption { get; }
            public string Name { get; }
            public string Value { get; }
        }
    }
}
