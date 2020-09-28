using MatthiWare.CommandLine.Abstractions;
using MatthiWare.CommandLine.Abstractions.Command;
using MatthiWare.CommandLine.Abstractions.Models;
using MatthiWare.CommandLine.Abstractions.Parsing;
using MatthiWare.CommandLine.Core.Command;
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

        public bool TryGetValue(IArgument argument, out ArgumentModel model)
        {
            throw new NotImplementedException();
        }

        public ArgumentManager2(CommandLineParserOptions options, ICommandLineCommandContainer commandContainer)
        {
            this.options = options ?? throw new ArgumentNullException(nameof(options));
            this.commandContainer = commandContainer ?? throw new ArgumentNullException(nameof(commandContainer));
        }

        public void Process(IReadOnlyList<string> arguments)
        {
            enumerator = new ArgumentRecordEnumerator(options, arguments);
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
                    return new OptionRecord(current, options.PostfixOption, isLongOption, options.PrefixShortOption.Length, options.PrefixLongOption.Length);
                }
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

        private sealed class OptionRecord : ArgumentRecord
        {
            public OptionRecord(string data, string postfix, bool isLongOption, int shortOptionLength,  int longOptionLength)
                : base(data)
            {
                var tokens = data.Split(postfix.ToCharArray()[0]);

                if (tokens.Length > 1)
                {
                    Value = tokens[1];
                }

                
                Name = tokens[0].Substring(isLongOption ? longOptionLength : shortOptionLength);
            }

            public string Name { get; set; }
            public string Value { get; set; }
        }
    }
}
