namespace MatthiWare.CommandLine.Abstractions.Usage
{
    /// <summary>
    /// Environment variables 
    /// </summary>
    public interface IEnvironmentVariablesService
    {
        /// <summary>
        /// Inidicates if NO_COLOR environment variable has been set
        /// https://no-color.org/
        /// </summary>
        bool NoColorRequested { get; }
    }
}
