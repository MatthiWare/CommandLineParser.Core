using MatthiWare.CommandLine.Abstractions.Command;
using MatthiWare.CommandLine.Abstractions.Parsing;
using MatthiWare.CommandLine.Core.Command;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using MatthiWare.CommandLine.Abstractions;

namespace MatthiWare.CommandLine.Core.Parsing
{
    internal class ArgumentManager : IArgumentGrouper, IDisposable
    {
        private IDictionary<ICommandLineCommand, string> arguments;

        public ArgumentManager(string[] args, List<CommandLineCommandBase> commands, List<CommandLineOptionBase> options)
        {
            arguments = new Dictionary<ICommandLineCommand, string>(commands.Count + options.Count);

            var lstArgs = new List<ArgumentValueHolder>(args.Select(arg => new ArgumentValueHolder
            {
                Argument = arg,
                Used = false
            }));

            Parse(lstArgs, commands);

            Parse(lstArgs, options);

            foreach (var item in lstArgs)
            {
                if (item.Used) continue;

                item.Used = true;

                arguments.Add(item.Command, item.Argument);
            }
        }

        private void Parse(List<ArgumentValueHolder> args, IEnumerable<ICommandLineCommand> list)
        {
            foreach (var item in list)
            {
                int idx = args.FindIndex(arg => !arg.Used && 
                    ((item.HasShortName && string.Equals(item.ShortName, arg.Argument, StringComparison.InvariantCultureIgnoreCase)) ||
                    (item.HasLongName && string.Equals(item.LongName, arg.Argument, StringComparison.InvariantCultureIgnoreCase))));

                args[idx].Used = true;
                args[idx].Command = item;
            }
        }

        public void Dispose()
        {
            arguments.Clear();
        }

        public bool TryGetValue(ICommandLineCommand cmd, out string value) => arguments.TryGetValue(cmd, out value);

        private class ArgumentValueHolder
        {
            public string Argument { get; set; }
            public bool Used { get; set; }
            public ICommandLineCommand Command { get; set; }
        }
    }
}
