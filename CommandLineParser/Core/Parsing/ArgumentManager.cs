using System;
using System.Collections.Generic;
using System.Linq;

using MatthiWare.CommandLine.Abstractions;
using MatthiWare.CommandLine.Abstractions.Models;
using MatthiWare.CommandLine.Abstractions.Parsing;
using MatthiWare.CommandLine.Core.Command;

namespace MatthiWare.CommandLine.Core.Parsing
{
    internal class ArgumentManager : IArgumentManager, IDisposable
    {
        private readonly IDictionary<ICommandLineOption, ArgumentModel> arguments;
        private readonly List<ArgumentValueHolder> args;

        public ArgumentManager(string[] args, ICollection<CommandLineCommandBase> commands, ICollection<CommandLineOptionBase> options)
        {
            arguments = new Dictionary<ICommandLineOption, ArgumentModel>(commands.Count + options.Count);

            this.args = new List<ArgumentValueHolder>(args.Select(arg => new ArgumentValueHolder
            {
                Argument = arg,
                Used = false
            }));

            ParseCommands(commands);

            ParseOptions(options);

            foreach (var item in this.args)
            {
                if (item.Option == null) continue;

                int nextIndex = item.Index + 1;

                var argValue = nextIndex < this.args.Count ? this.args[nextIndex] : null;

                var argModel = new ArgumentModel
                {
                    Key = item.Argument,
                    Value = (argValue?.Used ?? true) ? null : argValue.Argument
                };

                arguments.Add(item.Option, argModel);
            }
        }

        private void ParseOptions(IEnumerable<ICommandLineOption> options)
        {
            foreach (var option in options)
            {
                int idx = FindIndex(option);

                if (idx == -1) continue; // not found issue #12

                SetArgumentUsed(idx, option);
            }
        }

        private void ParseCommands(IEnumerable<CommandLineCommandBase> cmds)
        {
            foreach (var cmd in cmds)
            {
                int idx = FindIndex(cmd);

                if (idx == -1) continue;

                SetArgumentUsed(idx, cmd);

                foreach (var option in cmd.Options)
                {
                    // find the option index starting at the command index
                    int optionIdx = FindIndex(option, idx);

                    SetArgumentUsed(optionIdx, option);
                }
            }
        }

        private void SetArgumentUsed(int idx, ICommandLineOption option)
        {
            args[idx].Used = true;
            args[idx].Option = option;
            args[idx].Index = idx;
        }

        /// <summary>
        /// Finds the index of the first unused argument
        /// </summary>
        /// <param name="args">List of arguments to search</param>
        /// <param name="option">Option to find</param>
        /// <param name="startOffset">Search offset</param>
        /// <returns></returns>
        private int FindIndex(ICommandLineOption option, int startOffset = 0)
            => args.FindIndex(startOffset, arg => !arg.Used &&
                    ((option.HasShortName && string.Equals(option.ShortName, arg.Argument, StringComparison.InvariantCultureIgnoreCase)) ||
                    (option.HasLongName && string.Equals(option.LongName, arg.Argument, StringComparison.InvariantCultureIgnoreCase))));

        public void Dispose()
        {
            arguments.Clear();
        }

        public bool TryGetValue(ICommandLineOption option, out ArgumentModel model) => arguments.TryGetValue(option, out model);

        private class ArgumentValueHolder
        {
            public string Argument { get; set; }
            public bool Used { get; set; }
            public ICommandLineOption Option { get; set; }
            public int Index { get; set; }
        }
    }
}
