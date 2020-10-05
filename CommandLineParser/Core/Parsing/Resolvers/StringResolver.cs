using MatthiWare.CommandLine.Abstractions.Parsing;

namespace MatthiWare.CommandLine.Core.Parsing.Resolvers
{
    internal class StringResolver : BaseArgumentResolver<string>
    {
        public override bool CanResolve(string value) => value != null;

        public override string Resolve(string value) => value;
    }
}
