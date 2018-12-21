namespace MatthiWare.CommandLine.Abstractions.Command
{
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
        /// <param name="required">True or false</param>
        /// <returns><see cref="ICommandConfigurationBuilder"/></returns>
        ICommandConfigurationBuilder Description(string description);

        /// <summary>
        /// Configures the command name
        /// </summary>
        /// <param name="name">Short name</param>
        /// <returns><see cref="ICommandConfigurationBuilder"/></returns>
        ICommandConfigurationBuilder Name(string name);
    }
}
