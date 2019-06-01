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

        public void PrintCommandDescription(ICommandLineCommand command)
            => stringBuilder.AppendLine($"  {command.Name,-20}{command.Description,-50}");

        public void PrintCommandDescriptions(IEnumerable<ICommandLineCommand> commands)
        {
            if (!commands.Any()) return;

            stringBuilder.AppendLine().AppendLine("Commands: ");

            foreach (var cmd in commands)
                PrintCommandDescription(cmd);
        }

        public void PrintOption(ICommandLineOption option)
        {
            bool hasShort = option.HasShortName;
            bool hasLong = option.HasLongName;
            bool hasBoth = hasShort && hasLong;

            string hasBothSeparator = hasBoth ? optionSeparator : string.Empty;
            string shortName = hasShort ? option.ShortName : string.Empty;
            string longName = hasLong ? option.LongName : string.Empty;

            string key = $"{shortName}{hasBothSeparator}{longName}";

            stringBuilder.AppendLine($"  {key,-20}{option.Description,-50}");
        }

        public void PrintOptions(IEnumerable<ICommandLineOption> options)
        {
            if (!options.Any()) return;

            stringBuilder.AppendLine().AppendLine("Options: ");

            foreach (var opt in options)
            { 
                PrintOption(opt);
            }
        }
    }
}
