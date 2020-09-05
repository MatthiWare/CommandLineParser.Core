namespace MatthiWare.CommandLine.Abstractions.Models
{
    /// <summary>
    /// Wraps the resulting option model
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public interface IResultModelWrapper<TModel>
    {
        /// <summary>
        /// Gets or sets the result option model
        /// </summary>
        TModel Result { get; set; }
    }
}
