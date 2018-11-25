using System;
using System.Linq.Expressions;

namespace MatthiWare.CommandLine.Abstractions.Command
{
    public interface ICommandBuilder<Tsource> where Tsource : class, new()
    {
        ICommandBuilder<Tsource> Required(bool required = true);

        ICommandBuilder<Tsource> HelpText(string help);

        ICommandBuilder<Tsource> Name(string shortName);

        ICommandBuilder<Tsource> Name(string shortName, string longName);

        ICommandBuilder<Tsource> OnExecuting(Action<Tsource> action);

        IOptionBuilder Configure<TProperty>(Expression<Func<Tsource, TProperty>> selector);
    }
}
