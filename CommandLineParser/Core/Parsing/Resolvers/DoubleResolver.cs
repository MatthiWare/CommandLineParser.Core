using System.Globalization;
using MatthiWare.CommandLine.Abstractions.Parsing;

namespace MatthiWare.CommandLine.Core.Parsing.Resolvers
{
    internal class DoubleResolver : BaseArgumentResolver<double>
    {
        public override bool CanResolve(string value)
        {
            if (value is null)
            {
                return false;
            }

            return TryResolve(value, out _);
        }

        public override double Resolve(string value)
        {
            TryResolve(value, out double result);

            return result;
        }

        private bool TryResolve(string value, out double result)
        {
            return double.TryParse(value, NumberStyles.AllowExponent | NumberStyles.Number | NumberStyles.Float, CultureInfo.InvariantCulture, out result);
        }
    }
}
