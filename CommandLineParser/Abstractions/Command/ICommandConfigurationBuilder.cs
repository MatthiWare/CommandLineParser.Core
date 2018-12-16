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
        /// Configures the help text for the command
        /// </summary>
        /// <param name="required">True or false</param>
        /// <returns><see cref="ICommandConfigurationBuilder"/></returns>
        ICommandConfigurationBuilder HelpText(string help);

        /// <summary>
        /// Configures the command name
        /// </summary>
        /// <param name="shortName">Short name</param>
        /// <returns><see cref="ICommandConfigurationBuilder"/></returns>
        ICommandConfigurationBuilder Name(string shortName);

        /// <summary>
        /// Configures the command name
        /// </summary>
        /// <param name="shortName">Short name</param>
        /// <param name="longName">Long name</param>
        /// <returns><see cref="ICommandConfigurationBuilder"/></returns>
        ICommandConfigurationBuilder Name(string shortName, string longName);
    }
}
