namespace MatthiWare.CommandLine.Abstractions.Validations
{
    public abstract class ValidationConfigurationBase
    {
        protected IValidatorsContainer Validators { get; private set; }

        public ValidationConfigurationBase(IValidatorsContainer validators)
        {
            Validators = validators;
        }
    }
}
