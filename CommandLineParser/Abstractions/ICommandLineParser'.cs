using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using System.Text;
using MatthiWare.CommandLine.Abstractions.Parsing;

namespace MatthiWare.CommandLine.Abstractions
{
    public interface ICommandLineParser<T>
    {

        #region Properties

        IReadOnlyList<ICommandLineArgumentOption> Options { get; }
        IResolverFactory ResolverFactory { get; }

        #endregion

        #region Parsing

        IParserResult<T> Parse(string[] args);

        #endregion

        #region Configuration

        IOptionBuilder<TProperty> Configure<TProperty>(Expression<Func<T, TProperty>> selector);

        #endregion

    }
}
