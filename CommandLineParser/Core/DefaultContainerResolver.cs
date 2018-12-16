using System;
using MatthiWare.CommandLine.Abstractions;

namespace MatthiWare.CommandLine.Core
{
    internal class DefaultContainerResolver : IContainerResolver
    {
        public T Resolve<T>() => Activator.CreateInstance<T>();
    }
}
