using System;
using System.Threading;
using System.Threading.Tasks;

namespace MatthiWare.CommandLine.Abstractions.Command
{
    /// <summary>
    /// API for configurion command executions
    /// </summary>
    /// <typeparam name="TOption">Base option</typeparam>
    /// <typeparam name="TSource">Command option</typeparam>
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

        /// <summary>
        /// Sets the command execute action
        /// </summary>
        /// <param name="action">Action to execute</param>
        /// <returns><see cref="ICommandBuilder{TOption,TSource}"/></returns>
        ICommandBuilder<TOption, TSource> OnExecuting(Action action);

        /// <summary>
        /// Sets the command execute action
        /// </summary>
        /// <param name="action">Action to execute</param>
        /// <returns><see cref="ICommandBuilder{TOption,TSource}"/></returns>
        ICommandBuilder<TOption, TSource> OnExecuting(Action<TOption> action);

        /// <summary>
        /// Sets the command execute action
        /// </summary>
        /// <param name="action">Action to execute</param>
        /// <returns><see cref="ICommandBuilder{TOption,TSource}"/></returns>
        ICommandBuilder<TOption, TSource> OnExecuting(Action<TOption, TSource> action);

        /// <summary>
        /// Sets the async command execute action 
        /// </summary>
        /// <param name="action">Action to execute</param>
        /// <returns>A task of <see cref="ICommandBuilder{TOption,TSource}"/></returns>
        ICommandBuilder<TOption, TSource> OnExecutingAsync(Func<CancellationToken, Task> action);

        /// <summary>
        /// Sets the async command execute action
        /// </summary>
        /// <param name="action">Action to execute</param>
        /// <returns>A task of <see cref="ICommandBuilder{TOption,TSource}"/></returns>
        ICommandBuilder<TOption, TSource> OnExecutingAsync(Func<TOption, CancellationToken, Task> action);

        /// <summary>
        /// Sets the async command execute action
        /// </summary>
        /// <param name="action">Action to execute</param>
        /// <returns>A task of <see cref="ICommandBuilder{TOption,TSource}"/></returns>
        ICommandBuilder<TOption, TSource> OnExecutingAsync(Func<TOption, TSource, CancellationToken, Task> action);
    }
}
