using System.Threading;
using System.Threading.Tasks;

namespace MatthiWare.CommandLine.Abstractions.Command
{
    /// <summary>
    /// Defines a command
    /// </summary>
    public abstract class Command
    {
        /// <summary>
        /// Configures the command
        /// <see cref="ICommandConfigurationBuilder"/> for more info. 
        /// </summary>
        /// <param name="builder"></param>
        public virtual void OnConfigure(ICommandConfigurationBuilder builder) { }

        /// <summary>
        /// Executes the command
        /// </summary>
        public virtual void OnExecute() { }

        /// <summary>
        /// Executes the command async
        /// </summary>
        public virtual Task OnExecuteAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
