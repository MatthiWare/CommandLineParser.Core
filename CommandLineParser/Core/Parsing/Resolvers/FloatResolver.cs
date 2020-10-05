using MatthiWare.CommandLine.Abstractions.Parsing;
using System.Globalization;

namespace MatthiWare.CommandLine.Core.Parsing.Resolvers
{
    internal class FloatResolver : BaseArgumentResolver<float>
    {
        public override bool CanResolve(string value)
        {
            if (value is null)
            {
                return false;
            }

            return TryResolve(value, out _);
        }

        public override float Resolve(string value)
        {
            TryResolve(value, out float result);

            return result;
        }

        private bool TryResolve(string value, out float result)
        {
            return float.TryParse(value, NumberStyles.AllowExponent | NumberStyles.Number | NumberStyles.Float, CultureInfo.InvariantCulture, out result);
        }
    }
}
