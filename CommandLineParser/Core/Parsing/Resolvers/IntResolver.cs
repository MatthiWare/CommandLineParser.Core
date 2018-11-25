using MatthiWare.CommandLine.Abstractions.Models;
using MatthiWare.CommandLine.Abstractions.Parsing;

namespace MatthiWare.CommandLine.Core.Parsing.Resolvers
{
    public class IntResolver : ICommandLineArgumentResolver<int>
    {
        public bool CanResolve(ArgumentModel model)
        {
            if (!model.HasValue) return false;

            return TryResolve(model, out _);
        }

        public int Resolve(ArgumentModel model)
        {
            TryResolve(model, out int result);

            return result;
        }

        object ICommandLineArgumentResolver.Resolve(ArgumentModel model) => Resolve(model);

        private bool TryResolve(ArgumentModel model, out int result)
        {
            if (!model.HasValue)
            {
                result = 0;
                return false;
            }

            return int.TryParse(model.Value, out result);
        }
    }
}
