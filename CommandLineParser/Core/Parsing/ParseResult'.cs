using System;
using System.Collections.Generic;
using System.Linq;
using MatthiWare.CommandLine.Abstractions.Parsing;
using MatthiWare.CommandLine.Abstractions.Parsing.Command;

namespace MatthiWare.CommandLine.Core.Parsing
{
    internal class ParseResult<TResult> : IParserResult<TResult>
    {
        private readonly List<ICommandParserResult> commandParserResults = new List<ICommandParserResult>();
        private readonly ICollection<Exception> exceptions = new List<Exception>();
        private TResult result;

        #region Properties

        public TResult Result => result;

        public bool HasErrors { get; private set; } = false;

        public Exception Error
        {
            get
            {
                if (!HasErrors) return null;

                return (exceptions.Count > 1) ?
                    new AggregateException(exceptions) :
                    exceptions.First();
            }
        }

        public IReadOnlyList<ICommandParserResult> CommandResults => commandParserResults.AsReadOnly();

        #endregion

        public void MergeResult(ICommandParserResult result)
        {
            HasErrors |= result.HasErrors;

            commandParserResults.Add(result);
        }

        public void MergeResult(ICollection<Exception> errors)
        {
            if (!errors.Any()) return;

            HasErrors = true;

            foreach (var err in errors)
                exceptions.Add(err);
        }

        public void MergeResult(TResult result)
        {
            this.result = result;
        }

        public void ExecuteCommands()
        {
            if (HasErrors) throw new InvalidOperationException("Parsing failed commands might be corrupted.");

            foreach (var cmdResult in CommandResults)
                cmdResult.ExecuteCommand();
        }
    }
}
