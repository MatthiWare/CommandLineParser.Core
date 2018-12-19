using System.Diagnostics;

using MatthiWare.CommandLine.Abstractions;
using MatthiWare.CommandLine.Abstractions.Models;
using MatthiWare.CommandLine.Abstractions.Parsing;

namespace MatthiWare.CommandLine.Core
{
    [DebuggerDisplay("Cmd Option {ShortName ?? LongName}, Req: {IsRequired}, HasDefault: {HasDefault}")]
    internal abstract class CommandLineOptionBase : IParser, ICommandLineOption
    {
        public string ShortName { get; protected set; }
        public string LongName { get; protected set; }
        public string Description { get; protected set; }
        public bool IsRequired { get; protected set; }
        public bool HasDefault { get; protected set; }
        public bool HasShortName => !string.IsNullOrWhiteSpace(ShortName);
        public bool HasLongName => !string.IsNullOrWhiteSpace(LongName);

        public bool AutoExecute { get; protected set; }

        public abstract bool CanParse(ArgumentModel model);

        public abstract void Parse(ArgumentModel model);

        public abstract void UseDefault();
    }
}
