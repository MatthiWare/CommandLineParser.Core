using System;

namespace MatthiWare.CommandLine.Abstractions.Command
{
    public interface ICommandBuilder<TOption>
    {
        /// <summary>
        /// Configures if the command is required
        /// </summary>
        /// <param name="required">True or false</param>
        /// <returns><see cref="ICommandBuilder{Tsource}"/></returns>
        ICommandBuilder<TOption> Required(bool required = true);

        /// <summary>
        /// Configures the help text for the command
        /// </summary>
        /// <param name="required">True or false</param>
        /// <returns><see cref="ICommandBuilder{Tsource}"/></returns>
        ICommandBuilder<TOption> HelpText(string help);

        /// <summary>
        /// Configures the command name
        /// </summary>
        /// <param name="shortName">Short name</param>
        /// <returns><see cref="ICommandBuilder{Tsource}"/></returns>
        ICommandBuilder<TOption> Name(string shortName);

        /// <summary>
        /// Configures the command name
        /// </summary>
        /// <param name="shortName">Short name</param>
        /// <param name="longName">Long name</param>
        /// <returns><see cref="ICommandBuilder{Tsource}"/></returns>
        ICommandBuilder<TOption> Name(string shortName, string longName);

        /// <summary>
        /// Configures the execution of the command
        /// </summary>
        /// <param name="required">True or false</param>
        /// <returns><see cref="ICommandBuilder{Tsource}"/></returns>
        ICommandBuilder<TOption> OnExecuting(Action<TOption> action);
    }
}
