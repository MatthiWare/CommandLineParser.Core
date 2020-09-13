using System;
using System.Collections.Generic;
using System.Reflection;

namespace MatthiWare.CommandLine.Abstractions.Command
{
    /// <summary>
    /// Allows to discover <see cref="Command"/>'s from assemblies
    /// </summary>
    public interface ICommandDiscoverer
    {
        /// <summary>
        /// Discover commands
        /// </summary>
        /// <param name="optionType">Only commands with this option type are valid, or non generic commands</param>
        /// <param name="assemblies">List of assemblies to scan</param>
        /// <returns>A list of valid commands that need to be registered</returns>
        IReadOnlyList<Type> DiscoverCommandTypes(Type optionType, Assembly[] assemblies);
    }
}
