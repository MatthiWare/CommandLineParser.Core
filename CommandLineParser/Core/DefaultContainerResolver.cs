using System;

using MatthiWare.CommandLine.Abstractions;

namespace MatthiWare.CommandLine.Core
{
    internal class DefaultContainerResolver : IContainerResolver
    {
        public virtual T Resolve<T>() => Activator.CreateInstance<T>();

        public virtual object Resolve(Type type) => Activator.CreateInstance(type);
    }
}
