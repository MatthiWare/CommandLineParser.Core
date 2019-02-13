using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MatthiWare.CommandLine.Abstractions;
using MatthiWare.CommandLine.Abstractions.Command;
using MatthiWare.CommandLine.Abstractions.Usage;
using MatthiWare.CommandLine.Core.Command;
using MatthiWare.CommandLine.Core.Utils;

namespace MatthiWare.CommandLine.Core.Usage
{
    internal class UsageBuilder : IUsageBuilder
    {
        private readonly StringBuilder stringBuilder = new StringBuilder();
        private readonly CommandLineParserOptions parserOptions;
        private readonly string optionSeparator = "|";

        public UsageBuilder(CommandLineParserOptions parserOptions)
            => this.parserOptions = parserOptions;

        public void Print()
        {
            Console.WriteLine(stringBuilder.ToString());
            stringBuilder.Clear();
        }

        public void PrintUsage(string name, bool hasOptions, bool hasCommands)
        {
            stringBuilder.AppendLine()
                .Append("Usage: ")
                .Append(parserOptions.AppName)
                .AppendIf(!string.IsNullOrWhiteSpace(parserOptions.AppName), " ")
                .Append(name)
                .AppendIf(!string.IsNullOrWhiteSpace(name), " ")
                .Append(hasOptions ? "[options] " : string.Empty)
                .Append(hasCommands ? "[commands]" : string.Empty)
                .AppendLine();
        }

        public void PrintCommand(string name, ICommandLineCommandContainer container)
        {
            PrintUsage(name, container.Options.Any(), container.Commands.Any());

            PrintOptions(container.Options);

            PrintCommandDescriptions(container.Commands);
        }

        public void PrintCommandDescription(ICommandLineCommand command, int descriptionShift = 4)
            => stringBuilder.AppendLine($"  {command.Name}{new string(' ', descriptionShift)}{command.Description}");

        public void PrintCommandDescriptions(IEnumerable<ICommandLineCommand> commands, int descriptionShift = 4)
        {
            if (!commands.Any()) return;

            stringBuilder.AppendLine().AppendLine("Commands: ");

            var longestCommandName = commands.Max(x => x.Name.Length);
            foreach (var cmd in commands)
                PrintCommandDescription(cmd, longestCommandName - cmd.Name.Length + descriptionShift);
        }

        public void PrintOption(ICommandLineOption option, int descriptionShift = 4, bool compensateSeparator = false)
        {
            bool hasShort = option.HasShortName;
            bool hasLong = option.HasLongName;
            bool hasBoth = hasShort && hasLong;

            string hasBothSeparator = hasBoth ? optionSeparator : string.Empty;
            string shortName = hasShort ? option.ShortName : string.Empty;
            string longName = hasLong ? option.LongName : string.Empty;

            // We neeed to compensate a separator if given option doesn't have both (short & long) names.
            int indentationLength = descriptionShift + ((compensateSeparator && !hasBoth) ? optionSeparator.Length : 0);
            string indentation = new string(' ', indentationLength);

            stringBuilder.AppendLine($"  {shortName}{hasBothSeparator}{longName}{indentation}{option.Description}");
        }

        public void PrintOptions(IEnumerable<ICommandLineOption> options, int descriptionShift = 4)
        {
            if (!options.Any()) return;

            stringBuilder.AppendLine().AppendLine("Options: ");

            var longestOptionName = options.Max(x => (x.HasShortName ? x.ShortName.Length : 0) + (x.HasLongName ? x.LongName.Length : 0));
            var compensateSeparator = options.Any(x => x.HasShortName && x.HasLongName);

            foreach (var opt in options)
            {
                var longNameLength = opt.HasLongName ? opt.LongName.Length : 0;
                var shortNameLength = opt.HasShortName ? opt.ShortName.Length : 0;
                descriptionShift = longestOptionName - longNameLength - shortNameLength + descriptionShift;

                PrintOption(opt, descriptionShift, compensateSeparator);
            }
        }
    }
}
