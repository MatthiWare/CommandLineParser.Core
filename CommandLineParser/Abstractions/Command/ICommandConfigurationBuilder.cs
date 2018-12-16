namespace MatthiWare.CommandLine.Abstractions.Command
{
    public interface ICommandConfigurationBuilder<TOption, TSource>
    {
        /// <summary>
        /// Configures if the command is required
        /// </summary>
        /// <param name="required">True or false</param>
        /// <returns><see cref="ICommandBuilder{TOption, TSource}"/></returns>
        ICommandConfigurationBuilder<TOption, TSource> Required(bool required = true);

        /// <summary>
        /// Configures the help text for the command
        /// </summary>
        /// <param name="required">True or false</param>
        /// <returns><see cref="ICommandBuilder{TOption, TSource}"/></returns>
        ICommandConfigurationBuilder<TOption, TSource> HelpText(string help);

        /// <summary>
        /// Configures the command name
        /// </summary>
        /// <param name="shortName">Short name</param>
        /// <returns><see cref="ICommandBuilder{TOption, TSource}"/></returns>
        ICommandConfigurationBuilder<TOption, TSource> Name(string shortName);

        /// <summary>
        /// Configures the command name
        /// </summary>
        /// <param name="shortName">Short name</param>
        /// <param name="longName">Long name</param>
        /// <returns><see cref="ICommandBuilder{TOption, TSource}"/></returns>
        ICommandConfigurationBuilder<TOption, TSource> Name(string shortName, string longName);
    }
}
