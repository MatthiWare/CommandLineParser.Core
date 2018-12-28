using System;
using System.Collections.Generic;
using System.Text;

namespace MatthiWare.CommandLine.Abstractions.Command
{
    /// <summary>
    /// Defines a command
    /// </summary>
    /// <typeparam name="TOptions">Base options of the command</typeparam>
    public abstract class Command<TOptions>
        : Command
        where TOptions : class, new()
    {
        /// <summary>
        /// Executes the command
        /// </summary>
        /// <param name="options">Parsed options</param>
        public virtual void OnExecute(TOptions options)
        {
            OnExecute();
        }
    }
}
