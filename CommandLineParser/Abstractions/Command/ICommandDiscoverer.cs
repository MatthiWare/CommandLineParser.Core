using System;
using System.Collections.Generic;
using System.Reflection;

namespace MatthiWare.CommandLine.Abstractions.Command
{
    public interface ICommandDiscoverer
    {
        IReadOnlyList<Type> DiscoverCommandTypes(Type optionType, Assembly[] assemblies);
    }
}
