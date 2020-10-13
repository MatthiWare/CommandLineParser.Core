using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MatthiWare.CommandLine.Abstractions;
using MatthiWare.CommandLine.Abstractions.Command;
using MatthiWare.CommandLine.Abstractions.Usage;
using MatthiWare.CommandLine.Core.Utils;

namespace MatthiWare.CommandLine.Core.Usage
{
    /// <inheritdoc/>
    public class UsageBuilder : IUsageBuilder
    {
        private readonly StringBuilder stringBuilder = new StringBuilder();
        private readonly CommandLineParserOptions parserOptions;
        private readonly string optionSeparator = "|";

        /// <inheritdoc/>
        public UsageBuilder(CommandLineParserOptions parserOptions)
            => this.parserOptions = parserOptions;

        /// <inheritdoc/>
        public virtual string Build()
        {
            var result = stringBuilder.ToString();
            stringBuilder.Clear();
            return result;
        }

        /// <inheritdoc/>
        public virtual void AddUsage(string name, bool hasOptions, bool hasCommands)
        {
            stringBuilder
                .AppendLine()
                .Append("Usage: ")
                .Append(parserOptions.AppName)
                .AppendIf(!string.IsNullOrWhiteSpace(parserOptions.AppName), " ")
                .Append(name)
                .AppendIf(!string.IsNullOrWhiteSpace(name), " ")
                .Append(hasOptions ? "[options] " : string.Empty)
                .Append(hasCommands ? "[commands]" : string.Empty)
                .AppendLine();
        }

        /// <inheritdoc/>
        public virtual void AddCommand(string name, ICommandLineCommand command)
            => AddCommand(name, command as ICommandLineCommandContainer);

        /// <inheritdoc/>
        public virtual void AddCommand(string name, ICommandLineCommandContainer container)
        {
            AddUsage(name, container.Options.Any(), container.Commands.Any());

            AddOptions(container.Options);

            AddCommandDescriptions(container.Commands);
        }

        /// <inheritdoc/>
        public virtual void AddCommandDescription(ICommandLineCommand command)
            => stringBuilder.AppendLine($"  {command.Name,-20}{command.Description,-50}");

        /// <inheritdoc/>
        public virtual void AddCommandDescriptions(IEnumerable<ICommandLineCommand> commands)
        {
            if (!commands.Any())
            {
                return;
            }

            stringBuilder.AppendLine().AppendLine("Commands: ");

            foreach (var cmd in commands)
            {
                AddCommandDescription(cmd);
            }
        }

        /// <inheritdoc/>
        public virtual void AddOption(ICommandLineOption option)
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

        /// <inheritdoc/>
        public virtual void AddOptions(IEnumerable<ICommandLineOption> options)
        {
            if (!options.Any())
            {
                return;
            }

            stringBuilder
                .AppendLine()
                .AppendLine("Options: ");

            foreach (var opt in options)
            { 
                AddOption(opt);
            }
        }

        /// <inheritdoc/>
        public virtual void AddErrors(IReadOnlyCollection<Exception> errors)
        {
            foreach (var error in errors)
            {
                stringBuilder.AppendLine(error.Message);
            }
        }

        public void AddSuggestionHeader(string inputKey)
        {
            stringBuilder.AppendLine($"'{inputKey}' is not recognized as a valid command or option.");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine("Did you mean: ");
        }

        public void AddSuggestion(string suggestion)
        {
            stringBuilder.Append($"\t{suggestion}");
        }
    }
}
