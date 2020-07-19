using MatthiWare.CommandLine.Abstractions.Command;
using MatthiWare.CommandLine.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MatthiWare.CommandLine.Core.Command
{
    public class CommandDiscoverer : ICommandDiscoverer
    {
        public IReadOnlyList<Type> DiscoverCommandTypes(Assembly assembly)
            => DiscoverCommandTypes(new[] { assembly });

        public IReadOnlyList<Type> DiscoverCommandTypes(Assembly[] assemblies)
        {
            List<Type> foundCommands = new List<Type>();

            foreach (var assembly in assemblies)
            {
                FindCommandsInAssembly(assembly, foundCommands);
            }

            return foundCommands.AsReadOnly();
        }

        private void FindCommandsInAssembly(Assembly assembly, List<Type> list)
            => list.AddRange(assembly.ExportedTypes.Where(IsValidCommandType));

        private static bool IsValidCommandType(Type type)
        {
            if (type.IsAbstract || !type.IsClass)
            {
                return false;
            }

            return type.IsAssignableToGenericType(typeof(Command<>));
        }
    }
}
