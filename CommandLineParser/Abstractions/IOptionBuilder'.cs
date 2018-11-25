namespace MatthiWare.CommandLine.Abstractions
{
    public interface IOptionBuilder
    {
        IOptionBuilder Required(bool required = true);

        IOptionBuilder HelpText(string help);

        IOptionBuilder Default(object defaultValue);

        IOptionBuilder Name(string shortName);

        IOptionBuilder Name(string shortName, string longName);
    }
}
