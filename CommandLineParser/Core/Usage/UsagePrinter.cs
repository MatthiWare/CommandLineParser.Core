﻿using MatthiWare.CommandLine.Abstractions;
using MatthiWare.CommandLine.Abstractions.Command;
using MatthiWare.CommandLine.Abstractions.Usage;

namespace MatthiWare.CommandLine.Core.Usage
{
    internal class UsagePrinter : IUsagePrinter
    {
        private readonly CommandLineParserOptions m_parserOptions;
        private readonly IUsageBuilder m_usageBuilder;
        private readonly ICommandLineCommandContainer m_commandContainer;

        public UsagePrinter(CommandLineParserOptions parserOptions, ICommandLineCommandContainer container, IUsageBuilder builder)
        {
            m_parserOptions = parserOptions;
            m_commandContainer = container;

            m_usageBuilder = builder;
        }

        public void PrintUsage()
        {
            m_usageBuilder.PrintCommand(string.Empty, m_commandContainer);
            m_usageBuilder.Print();
        }

        public void PrintUsage(IArgument argument)
        {
            switch (argument)
            {
                case ICommandLineCommand cmd:
                    PrintUsage(cmd);
                    break;
                case ICommandLineOption opt:
                    PrintUsage(opt);
                    break;
                default:
                    PrintUsage();
                    break;
            }
        }

        public void PrintUsage(ICommandLineCommand command)
        {
            m_usageBuilder.PrintCommand(command.Name, (ICommandLineCommandContainer)command);
            m_usageBuilder.Print();
        }

        public void PrintUsage(ICommandLineOption option)
        {
            m_usageBuilder.PrintOption(option);
            m_usageBuilder.Print();
        }
    }
}
