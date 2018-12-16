namespace MatthiWare.CommandLine.Abstractions.Command
{
    public abstract class Command<TOptions>
        where TOptions : class, new()
    {

        public virtual void OnConfigure(ICommandConfigurationBuilder builder) { }

        public virtual void OnExecute(TOptions options) { }
    }

    public abstract class Command<TOptions, TCommandOptions> :
        Command<TOptions>
        where TOptions : class, new()
        where TCommandOptions : class, new()
    {
        public virtual void OnExecute(TOptions options, TCommandOptions commandOptions) { }
    }
}
