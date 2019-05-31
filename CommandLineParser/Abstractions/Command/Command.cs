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
        /// <param name="options">Parsed options</param>
        public virtual void OnExecute() { }
    }
}
