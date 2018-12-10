using System;
using System.Collections.Generic;
using System.Linq;
using MatthiWare.CommandLine.Abstractions.Command;
using MatthiWare.CommandLine.Abstractions.Models;
using MatthiWare.CommandLine.Abstractions.Parsing;
using MatthiWare.CommandLine.Core.Command;

namespace MatthiWare.CommandLine.Core.Parsing
{
    internal class ArgumentManager : IArgumentManager, IDisposable
    {
        private readonly IDictionary<ICommandLineCommand, ArgumentModel> arguments;
        private readonly List<ArgumentValueHolder> args;

        public ArgumentManager(string[] args, ICollection<CommandLineCommandBase> commands, ICollection<CommandLineOptionBase> options)
        {
            arguments = new Dictionary<ICommandLineCommand, ArgumentModel>(commands.Count + options.Count);

            this.args = new List<ArgumentValueHolder>(args.Select(arg => new ArgumentValueHolder
            {
                Argument = arg,
                Used = false
            }));

            ParseCommands(commands);

            Parse(options);

            foreach (var item in this.args)
            {
                if (item.Command == null) continue;

                int nextIndex = item.Index + 1;

                var argValue = nextIndex < this.args.Count ? this.args[nextIndex] : null;

                var argModel = new ArgumentModel
                {
                    Key = item.Argument,
                    Value = (argValue?.Used ?? true) ? null : argValue.Argument
                };

                arguments.Add(item.Command, argModel);
            }
        }

        private void Parse(IEnumerable<ICommandLineCommand> list)
        {
            foreach (var item in list)
            {
                int idx = FindIndex(item);

                if (idx == -1) continue; // not found issue #12

                SetArgumentUsed(idx, item);
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

        private void SetArgumentUsed(int idx, ICommandLineCommand cmd)
        {
            args[idx].Used = true;
            args[idx].Command = cmd;
            args[idx].Index = idx;
        }

        /// <summary>
        /// Finds the index of the first unused argument
        /// </summary>
        /// <param name="args">List of arguments to search</param>
        /// <param name="cmd">Option to find</param>
        /// <param name="startOffset">Search offset</param>
        /// <returns></returns>
        private int FindIndex(ICommandLineCommand cmd, int startOffset = 0)
            => args.FindIndex(startOffset, arg => !arg.Used &&
                    ((cmd.HasShortName && string.Equals(cmd.ShortName, arg.Argument, StringComparison.InvariantCultureIgnoreCase)) ||
                    (cmd.HasLongName && string.Equals(cmd.LongName, arg.Argument, StringComparison.InvariantCultureIgnoreCase))));

        public void Dispose()
        {
            arguments.Clear();
        }

        public bool TryGetValue(ICommandLineCommand cmd, out ArgumentModel model) => arguments.TryGetValue(cmd, out model);

        private class ArgumentValueHolder
        {
            public string Argument { get; set; }
            public bool Used { get; set; }
            public ICommandLineCommand Command { get; set; }
            public int Index { get; set; }
        }
    }
}
