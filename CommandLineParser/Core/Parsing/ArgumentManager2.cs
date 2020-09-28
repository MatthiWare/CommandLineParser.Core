using MatthiWare.CommandLine.Abstractions;
using MatthiWare.CommandLine.Abstractions.Command;
using MatthiWare.CommandLine.Abstractions.Models;
using MatthiWare.CommandLine.Abstractions.Parsing;
using MatthiWare.CommandLine.Core.Command;
using MatthiWare.CommandLine.Core.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MatthiWare.CommandLine.Core.Parsing
{
    public class ArgumentManager2 : IArgumentManager
    {
        private readonly CommandLineParserOptions options;
        private readonly ICommandLineCommandContainer commandContainer;
        private IEnumerator<ArgumentRecord> enumerator;
        private Dictionary<IArgument, ArgumentModel> results;

        private ProcessingContext CurrentContext { get; set; }

        public bool TryGetValue(IArgument argument, out ArgumentModel model) => results.TryGetValue(argument, out model);

        public ArgumentManager2(CommandLineParserOptions options, ICommandLineCommandContainer commandContainer)
        {
            this.options = options ?? throw new ArgumentNullException(nameof(options));
            this.commandContainer = commandContainer ?? throw new ArgumentNullException(nameof(commandContainer));
        }

        public void Process(IReadOnlyList<string> arguments)
        {
            results = new Dictionary<IArgument, ArgumentModel>();
            enumerator = new ArgumentRecordEnumerator(options, arguments);
            CurrentContext = new ProcessingContext(null, commandContainer);

            while (enumerator.MoveNext())
            {
                ProcessNext();
            }
        }

        private void ProcessNext()
        {
            switch (enumerator.Current)
            {
                case OptionRecord option:
                    ProcessOption(option);
                    break;
                case CommandOrOptionValueRecord commandOrValue:
                    ProcessCommandOrOptionValue(commandOrValue);
                    break;
            }
        }

        private void ProcessOption(OptionRecord rec)
        {
            foreach (var option in CurrentContext.CurrentCommand.Options)
            {
                if (!rec.Name.EqualsIgnoreCase(rec.IsLongOption ? option.LongName : option.ShortName))
                {
                    continue;
                }

                var argumentModel = new ArgumentModel(rec.Name, rec.Value);

                results.Add(option, argumentModel);

                CurrentContext.CurrentOption = option;

                break;
            }
        }

        private void ProcessCommandOrOptionValue(CommandOrOptionValueRecord rec)
        {
            foreach (var cmd in CurrentContext.CurrentCommand.Commands)
            {
                if (!rec.RawData.EqualsIgnoreCase(cmd.Name))
                {
                    continue;
                }

                results.Add(cmd, new ArgumentModel(cmd.Name, null));

                CurrentContext.CurrentCommand = (ICommandLineCommandContainer)cmd;

                return;
            }

            if (CurrentContext.CurrentOption == null)
            {
                return;
            }

            if (!TryGetValue(CurrentContext.CurrentOption, out var model))
            {
                // not sure yet what to do here.. 
                // no option yet and not matching command => unknown item
                return;
            }

            if (model.HasValue)
            {
                throw new ArgumentException("model already has a value????");
            }

            model.Value = rec.RawData;
        }

        private IEnumerable<ICommandLineOption> GetOptions(IEnumerable<ICommandLineOption> options, IEnumerable<CommandLineCommandBase> commands)
        {
            foreach (var option in options)
            {
                yield return option;
            }

            foreach (var command in commands)
            {
                foreach (var cmdOption in GetOptions(command.Options, command.Commands.Cast<CommandLineCommandBase>()))
                {
                    yield return cmdOption;
                }
            }
        }

        private class ProcessingContext
        {

            public ICommandLineOption CurrentOption { get; set; }
            public ProcessingContext Parent { get; set; }
            public ICommandLineCommandContainer CurrentCommand { get; set; }

            public ProcessingContext(ProcessingContext parent, ICommandLineCommandContainer commandContainer)
            {
                Parent = parent;
                CurrentCommand = commandContainer;
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

            public bool IsLongOption { get;  }
            public string Name { get; }
            public string Value { get; }
        }
    }
}
