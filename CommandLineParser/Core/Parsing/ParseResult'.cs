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

        public static ParseResult<T> FromResult<T>(T result)
            => new ParseResult<T>() { Result = result };

        public static ParseResult<T> FromError<T>(Exception error)
            => new ParseResult<T>() { Error = error };
    }
}
