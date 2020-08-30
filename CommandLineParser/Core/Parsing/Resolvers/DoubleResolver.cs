using System.Globalization;

using MatthiWare.CommandLine.Abstractions.Models;
using MatthiWare.CommandLine.Abstractions.Parsing;

namespace MatthiWare.CommandLine.Core.Parsing.Resolvers
{
    public class DoubleResolver : BaseArgumentResolver<double>
    {
        public override bool CanResolve(ArgumentModel model)
        {
            if (!model.HasValue) return false;

            return TryResolve(model, out _);
        }

        public override double Resolve(ArgumentModel model)
        {
            TryResolve(model, out double result);

            return result;
        }

        private bool TryResolve(ArgumentModel model, out double result)
        {
            if (!model.HasValue)
            {
                result = 0;
                return false;
            }

            return double.TryParse(model.Value, NumberStyles.AllowExponent | NumberStyles.Number | NumberStyles.Float, CultureInfo.InvariantCulture, out result);
        }
    }
}
