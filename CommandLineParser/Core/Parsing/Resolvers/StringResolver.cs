using MatthiWare.CommandLine.Abstractions.Models;
using MatthiWare.CommandLine.Abstractions.Parsing;

namespace MatthiWare.CommandLine.Core.Parsing.Resolvers
{
    internal class StringResolver : BaseArgumentResolver<string>
    {
        public override bool CanResolve(ArgumentModel model) => model.HasValue;

        public override string Resolve(ArgumentModel model) => model.HasValue ? model.Value : null;
    }
}
