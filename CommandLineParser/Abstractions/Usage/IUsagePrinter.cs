using MatthiWare.CommandLine.Abstractions.Command;

namespace MatthiWare.CommandLine.Abstractions.Usage
{
    public interface IUsagePrinter
    {
        void PrintUsage();
        void PrintUsage(ICommandLineCommand command);
        void PrintUsage(ICommandLineOption option);
    }
}
