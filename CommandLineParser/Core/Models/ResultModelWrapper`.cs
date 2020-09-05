using MatthiWare.CommandLine.Abstractions.Models;

namespace MatthiWare.CommandLine.Core.Models
{
    /// <inheritdoc/>
    public class ResultModelWrapper<TModel> : IResultModelWrapper<TModel>
    {
        /// <inheritdoc/>
        public TModel Result { get; set; }
    }
}
