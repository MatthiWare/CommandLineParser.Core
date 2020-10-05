using MatthiWare.CommandLine.Abstractions.Parsing;
using System.Globalization;

namespace MatthiWare.CommandLine.Core.Parsing.Resolvers
{
    internal class DecimalResolver : BaseArgumentResolver<decimal>
    {
        public override bool CanResolve(string value)
        {
            if (value is null)
            {
                return false;
            }

            return TryResolve(value, out _);
        }

        public override decimal Resolve(string value)
        {
            TryResolve(value, out decimal result);

            return result;
        }

        private bool TryResolve(string value, out decimal result)
        {
            return decimal.TryParse(value, NumberStyles.AllowExponent | NumberStyles.Number | NumberStyles.Float, CultureInfo.InvariantCulture, out result);
        }
    }
}
