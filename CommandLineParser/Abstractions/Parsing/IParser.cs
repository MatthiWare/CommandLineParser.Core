using MatthiWare.CommandLine.Abstractions.Models;

namespace MatthiWare.CommandLine.Abstractions.Parsing
{
    public interface IParser
    {
        bool CanParse(ArgumentModel model);
        void Parse(ArgumentModel model);
    }
}
