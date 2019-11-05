using MatthiWare.CommandLine.Abstractions.Command;

namespace FluentValidationsExtensions.Tests.Commands
{
    public class CommandWithModel<TBaseModel, TCommandModel> : Command<TBaseModel, TCommandModel>
        where TBaseModel : class, new()
        where TCommandModel : class, new()
    {
        public override void OnConfigure(ICommandConfigurationBuilder<TCommandModel> builder)
        {
            base.OnConfigure(builder);

            builder.Name("cmd");
        }

        public override void OnConfigure(ICommandConfigurationBuilder builder)
        {
            base.OnConfigure(builder);
        }
    }
}
