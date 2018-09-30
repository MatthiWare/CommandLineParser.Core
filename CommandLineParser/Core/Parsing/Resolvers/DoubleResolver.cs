using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using MatthiWare.CommandLine.Abstractions.Models;
using MatthiWare.CommandLine.Abstractions.Parsing;

namespace MatthiWare.CommandLine.Core.Parsing.Resolvers
{
    public class DoubleResolver : ICommandLineArgumentResolver<double>
    {
        public bool CanResolve(ArgumentModel model)
        {
            if (!model.HasValue) return false;

            return TryResolve(model, out _);
        }

        public double Resolve(ArgumentModel model)
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
