﻿namespace MatthiWare.CommandLine.Abstractions.Command
{
    /// <summary>
    /// Defines a command
    /// </summary>
    /// <typeparam name="TOptions">Base options of the command</typeparam>
    public abstract class Command<TOptions>
        where TOptions : class, new()
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
        public virtual void OnExecute(TOptions options) { }
    }

    /// <summary>
    /// Defines a command
    /// </summary>
    /// <typeparam name="TOptions">Base options of the command</typeparam>
    /// <typeparam name="TCommandOptions">Command options</typeparam>
    public abstract class Command<TOptions, TCommandOptions> :
        Command<TOptions>
        where TOptions : class, new()
        where TCommandOptions : class, new()
    {

        /// <summary>
        /// Executes the command
        /// </summary>
        /// <param name="options"></param>
        /// <param name="commandOptions"></param>
        public virtual void OnExecute(TOptions options, TCommandOptions commandOptions) { }
    }
}
