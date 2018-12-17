namespace MatthiWare.CommandLine.Abstractions
{
    public interface IContainerResolver
    {
        T Resolve<T>();
    }
}
