using System;
using System.Threading;
using System.Threading.Tasks;

namespace MatthiWare.CommandLine.Abstractions.Command
{
    /// <summary>
    /// Generic command builder
    /// </summary>
    /// <typeparam name="TOption"></typeparam>
    public interface ICommandBuilder<TOption>
    {
        /// <summary>
        /// Configures how the command should be invoked.
        /// Default behavior is to auto invoke the command.
        /// </summary>
        /// <param name="invoke">True if the command executor will be invoked (default), false if you want to invoke manually.</param>
        /// <returns><see cref="ICommandBuilder{TOption}"/></returns>
        ICommandBuilder<TOption> InvokeCommand(bool invoke);

        /// <summary>
        /// Configures if the command is required
        /// </summary>
        /// <param name="required">True or false</param>
        /// <returns><see cref="ICommandBuilder{TOption}"/></returns>
        ICommandBuilder<TOption> Required(bool required = true);

        /// <summary>
        /// Describes the command, used in the usage output. 
        /// </summary>
        /// <param name="description">description of the command</param>
        /// <returns><see cref="ICommandBuilder{TOption}"/></returns>
        ICommandBuilder<TOption> Description(string description);

        /// <summary>
        /// Configures the command name
        /// </summary>
        /// <param name="name">name</param>
        /// <returns><see cref="ICommandBuilder{TOption}"/></returns>
        ICommandBuilder<TOption> Name(string name);

        /// <summary>
        /// Configures the execution of the command
        /// </summary>
        /// <param name="action">The execution action</param>
        /// <returns><see cref="ICommandBuilder{TOption}"/></returns>
        ICommandBuilder<TOption> OnExecuting(Action<TOption> action);

        /// <summary>
        /// Configures the execution of the command async
        /// </summary>
        /// <param name="action">The execution action</param>
        /// <returns><see cref="ICommandBuilder{TOption}"/></returns>
        ICommandBuilder<TOption> OnExecutingAsync(Func<TOption, CancellationToken, Task> action);
    }
}
