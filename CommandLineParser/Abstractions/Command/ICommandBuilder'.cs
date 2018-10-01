using System;
using System.Linq.Expressions;

namespace MatthiWare.CommandLine.Abstractions.Command
{
    public interface ICommandBuilder<Tsource>
    {
        ICommandBuilder<Tsource> Required(bool required = true);

        ICommandBuilder<Tsource> HelpText(string help);

        ICommandBuilder<Tsource> ShortName(string shortName);

        ICommandBuilder<Tsource> LongName(string longName);

        ICommandBuilder<Tsource> OnExecuting(Action<Tsource> action);

        IOptionBuilder<TProperty> Configure<TProperty>(Expression<Func<Tsource, TProperty>> selector);
    }
}
