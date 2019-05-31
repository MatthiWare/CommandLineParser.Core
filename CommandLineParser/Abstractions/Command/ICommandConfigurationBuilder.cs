namespace MatthiWare.CommandLine.Abstractions.Command
{
    /// <summary>
    /// Command builder
    /// </summary>
    public interface ICommandConfigurationBuilder
    {
        /// <summary>
        /// Configures if the command is required
        /// </summary>
        /// <param name="required">True or false</param>
        /// <returns><see cref="ICommandConfigurationBuilder"/></returns>
        ICommandConfigurationBuilder Required(bool required = true);

        /// <summary>
        /// Configures the description text for the command
        /// </summary>
        /// <param name="description">Description</param>
        /// <returns><see cref="ICommandConfigurationBuilder"/></returns>
        ICommandConfigurationBuilder Description(string description);

        /// <summary>
        /// Configures the command name
        /// </summary>
        /// <param name="name">Command name</param>
        /// <returns><see cref="ICommandConfigurationBuilder"/></returns>
        ICommandConfigurationBuilder Name(string name);

        /// <summary>
        /// Configures if the command should auto execute
        /// </summary>
        /// <param name="autoExecute">True for automated execution, false for manual</param>
        /// <returns><see cref="ICommandConfigurationBuilder"/></returns>
        ICommandConfigurationBuilder AutoExecute(bool autoExecute);
    }
}
