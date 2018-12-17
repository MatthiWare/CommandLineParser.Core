using System;

namespace MatthiWare.CommandLine.Abstractions.Command
{
    public interface ICommandExecutor<TOption, TSource>
        where TOption : class
        where TSource : class, new()
    {
        /// <summary>
        /// Configures how the command should be invoked.
        /// Default behavior is to auto invoke the command.
        /// </summary>
        /// <param name="invoke">True if the command executor will be invoked (default), false if you want to invoke manually.</param>
        /// <returns><see cref="ICommandBuilder{TOption,TSource}"/></returns>
        ICommandBuilder<TOption, TSource> InvokeCommand(bool invoke);

        ICommandBuilder<TOption, TSource> OnExecuting(Action<TOption> action);

        ICommandBuilder<TOption, TSource> OnExecuting(Action<TOption, TSource> action);
    }
}
