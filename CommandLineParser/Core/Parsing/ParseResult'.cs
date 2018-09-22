using System;
using System.Collections.Generic;
using System.Text;
using MatthiWare.CommandLine.Abstractions.Parsing;

namespace MatthiWare.CommandLine.Core.Parsing
{
    public class ParseResult<TResult> : IParserResult<TResult>
    {

        #region Properties

        public TResult Result { get; private set; }

        public bool HasErrors => Error != null;

        public Exception Error { get; private set; }

        #endregion

        private ParseResult() { }

        public static ParseResult<TResult> FromResult(TResult result)
            => new ParseResult<TResult>() { Result = result };

        public static ParseResult<TResult> FromError(Exception error)
            => new ParseResult<TResult>() { Error = error };
    }
}
