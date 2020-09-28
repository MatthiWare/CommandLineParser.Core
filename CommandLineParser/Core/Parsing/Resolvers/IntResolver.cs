using MatthiWare.CommandLine.Abstractions.Models;
using MatthiWare.CommandLine.Abstractions.Parsing;

namespace MatthiWare.CommandLine.Core.Parsing.Resolvers
{
    internal class IntResolver : BaseArgumentResolver<int>
    {
        public override bool CanResolve(ArgumentModel model)
        {
            if (!model.HasValue)
            {
                return false;
            }

            return TryResolve(model, out _);
        }

        public override int Resolve(ArgumentModel model)
        {
            TryResolve(model, out int result);

            return result;
        }

        private bool TryResolve(ArgumentModel model, out int result)
        {
            if (model is null)
            {
                result = 0;
                return false;
            }

            if (!model.HasValue)
            {
                result = 0;
                return false;
            }

            return int.TryParse(model.Value, out result);
        }
    }
}
