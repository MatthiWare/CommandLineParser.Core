using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using MatthiWare.CommandLine.Abstractions.Command;
using MatthiWare.CommandLine.Abstractions.Parsing;

namespace MatthiWare.CommandLine.Abstractions
{
    public interface ICommandLineParser<TSource>
    {

        #region Properties

        IReadOnlyList<ICommandLineCommand> Commands { get; }
        IReadOnlyList<ICommandLineOption> Options { get; }
        IResolverFactory ResolverFactory { get; }

        #endregion

        #region Parsing

        IParserResult<TSource> Parse(string[] args);

        #endregion

        #region Configuration

        IOptionBuilder Configure<TProperty>(Expression<Func<TSource, TProperty>> selector);

        ICommandBuilder<TCommandOption> AddCommand<TCommandOption>() where TCommandOption : class, new();

        #endregion

    }
}
