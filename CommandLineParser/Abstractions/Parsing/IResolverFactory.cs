using System;

namespace MatthiWare.CommandLine.Abstractions.Parsing
{
    public interface IResolverFactory
    {
        bool Contains<T>();

        bool Contains(Type argument);

        void Register<TArgument, TResolver>(bool overwrite = false) where TResolver : ICommandLineArgumentResolver<TArgument>;

        void Register(Type argument, Type resolver, bool overwrite = false);

        ICommandLineArgumentResolver<T> CreateResolver<T>();

        ICommandLineArgumentResolver CreateResolver(Type type);
    }
}
