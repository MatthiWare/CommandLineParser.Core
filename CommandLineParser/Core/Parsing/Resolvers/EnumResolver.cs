using System;

using MatthiWare.CommandLine.Abstractions.Models;
using MatthiWare.CommandLine.Abstractions.Parsing;

namespace MatthiWare.CommandLine.Core.Parsing.Resolvers
{
    internal class EnumResolver<TEnum> : BaseArgumentResolver<TEnum>
        where TEnum : struct, Enum
    {
        public override bool CanResolve(ArgumentModel model)
            => TryResolve(model, out _);

        public override TEnum Resolve(ArgumentModel model)
        {
            TryResolve(model, out TEnum result);

            return result;
        }

        private bool TryResolve(ArgumentModel model, out TEnum result)
        {
            if (!model.HasValue)
            {
                result = default;
                return false;
            }

            return Enum.TryParse(model.Value, true, out result);
        }
    }
}
