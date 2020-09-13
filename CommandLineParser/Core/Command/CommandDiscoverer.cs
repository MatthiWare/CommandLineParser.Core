using MatthiWare.CommandLine.Abstractions.Command;
using MatthiWare.CommandLine.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MatthiWare.CommandLine.Core.Command
{
    /// <inheritdoc/>
    public class CommandDiscoverer : ICommandDiscoverer
    {
        /// <inheritdoc/>
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

            bool isAssignableToGenericCommand = type.IsAssignableToGenericType(typeof(Command<>));
            bool isAssignableToCommand = typeof(Abstractions.Command.Command).IsAssignableFrom(type);

            if (!isAssignableToCommand && !isAssignableToGenericCommand)
            {
                return false;
            }

            if (isAssignableToGenericCommand)
            {
                var firstGenericArgument = type.BaseType.GenericTypeArguments.First();

                if (optionType != firstGenericArgument)
                {
                    return false;
                }
            }
            
            return true;
        }
    }
}
