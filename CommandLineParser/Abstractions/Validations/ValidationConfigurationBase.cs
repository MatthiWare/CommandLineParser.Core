namespace MatthiWare.CommandLine.Abstractions.Validations
{
    /// <summary>
    /// Base validation configuration provider. 
    /// Needs to be inherited when implementing a specific validations provider
    /// </summary>
    public abstract class ValidationConfigurationBase
    {
        /// <summary>
        /// Gets a container of all validators
        /// </summary>
        protected IValidatorsContainer Validators { get; private set; }

        /// <summary>
        /// Create instance of the validation configuration base
        /// </summary>
        /// <param name="validators">validator container</param>
        public ValidationConfigurationBase(IValidatorsContainer validators)
        {
            Validators = validators;
        }
    }
}
