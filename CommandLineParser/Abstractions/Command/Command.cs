namespace MatthiWare.CommandLine.Abstractions.Command
{
    public abstract class Command<TOptions, TCommandOptions>
        where TOptions : class, new()
        where TCommandOptions : class, new()
    {

        public virtual void OnConfigure(IOptionBuilder builder) { }

        public virtual void OnExecute(TOptions options, TCommandOptions commandOptions) { }
    }
}
