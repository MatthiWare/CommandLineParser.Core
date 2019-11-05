using System;

namespace MatthiWare.CommandLine.Abstractions
{
    public interface IContainerResolver
    {
        T Resolve<T>();
        object Resolve(Type type);
    }
}
