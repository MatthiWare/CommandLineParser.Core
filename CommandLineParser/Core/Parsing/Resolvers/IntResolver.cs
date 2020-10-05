using MatthiWare.CommandLine.Abstractions.Parsing;

namespace MatthiWare.CommandLine.Core.Parsing.Resolvers
{
    internal class IntResolver : BaseArgumentResolver<int>
    {
        public override bool CanResolve(string value)
        {
            if (value is null)
            {
                return false;
            }

            return int.TryParse(value, out _);
        }

        public override int Resolve(string value)
        {
            int.TryParse(value, out int result);

            return result;
        }
    }
}
