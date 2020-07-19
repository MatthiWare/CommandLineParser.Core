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
        public IReadOnlyList<Type> DiscoverCommandTypes(Type optionType, Assembly[] assemblies)
        {
            var foundCommands = new List<Type>();

            foreach (var assembly in assemblies)
            {
                FindCommandsInAssembly(assembly, foundCommands, optionType);
            }

            return foundCommands.AsReadOnly();
        }

        private void FindCommandsInAssembly(Assembly assembly, List<Type> list, Type optionType)
            => list.AddRange(assembly.ExportedTypes.Where(t => IsValidCommandType(t, optionType)));

        private static bool IsValidCommandType(Type type, Type optionType)
        {
            if (type.IsAbstract || !type.IsClass)
            {
                return false;
            }

            if (!type.IsAssignableToGenericType(typeof(Command<>)))
            {
                return false;
            }

            var firstGenericArgument = type.BaseType.GenericTypeArguments.First();

            if (optionType != firstGenericArgument)
            {
                return false;
            }

            return true;
        }
    }
}
