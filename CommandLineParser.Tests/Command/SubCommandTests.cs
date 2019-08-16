using MatthiWare.CommandLine;
using MatthiWare.CommandLine.Abstractions;
using MatthiWare.CommandLine.Abstractions.Command;
using MatthiWare.CommandLine.Core.Attributes;
using System;
using System.Threading;
using Xunit;

namespace MatthiWare.CommandLine.Tests.Command
{
    public class SubCommandTests
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void TestSubCommandWorksCorrectlyInModel(bool autoExecute)
        {
            var lock1 = new ManualResetEventSlim();
            var lock2 = new ManualResetEventSlim();

            var containerResolver = new CustomInstantiator(lock1, lock2, autoExecute);

            var parser = new CommandLineParser<MainModel>(containerResolver);

            var result = parser.Parse(new[] { "main", "-b", "something", "sub", "-i", "15", "-n", "-1" });

            result.AssertNoErrors();

            if (!autoExecute)
                result.ExecuteCommands();

            Assert.True(lock1.Wait(1000), "MainCommand didn't execute in time.");
            Assert.True(lock2.Wait(1000), "SubCommand didn't execute in time.");
        }

        private class CustomInstantiator : IContainerResolver
        {
            private readonly ManualResetEventSlim lock1;
            private readonly ManualResetEventSlim lock2;
            private readonly bool autoExecute;

            public CustomInstantiator(ManualResetEventSlim lock1, ManualResetEventSlim lock2, bool autoExecute)
            {
                this.lock1 = lock1;
                this.lock2 = lock2;
                this.autoExecute = autoExecute;
            }

            public T Resolve<T>()
            {
                if (typeof(T) == typeof(MainCommand))
                    return (T)Activator.CreateInstance(typeof(T), lock1, autoExecute);
                else if (typeof(T) == typeof(SubCommand))
                    return (T)Activator.CreateInstance(typeof(T), lock2, autoExecute);
                else
                    return default;
            }
        }
    }

    public class MainCommand : Command<MainModel, SubModel>
    {
        private readonly ManualResetEventSlim locker;
        private readonly bool autoExecute;

        public MainCommand(ManualResetEventSlim locker, bool autoExecute)
        {
            this.locker = locker;
            this.autoExecute = autoExecute;
        }

        public override void OnConfigure(ICommandConfigurationBuilder<SubModel> builder)
        {
            builder
                .Name("main")
                .AutoExecute(autoExecute)
                .Required();
        }

        public override void OnExecute(MainModel options, SubModel commandOptions)
        {
            base.OnExecute(options, commandOptions);

            locker.Set();
        }
    }

    public class SubCommand : Command<MainModel, SubSubModel>
    {
        private readonly ManualResetEventSlim locker;
        private readonly bool autoExecute;

        public SubCommand(ManualResetEventSlim locker, bool autoExecute)
        {
            this.locker = locker;
            this.autoExecute = autoExecute;
        }

        public override void OnConfigure(ICommandConfigurationBuilder<SubSubModel> builder)
        {
            builder
                .Name("sub")
                .AutoExecute(autoExecute)
                .Required();
        }

        public override void OnExecute(MainModel options, SubSubModel commandOptions)
        {
            base.OnExecute(options, commandOptions);

            locker.Set();
        }
    }

    public class MainModel
    {
        [Required, Name("b")]
        public string Bla { get; set; }
        public MainCommand MainCommand { get; set; }
    }

    public class SubModel
    {
        [Required, Name("i")]
        public int Item { get; set; }
        public SubCommand SubCommand { get; set; }
    }

    public class SubSubModel
    {
        [Required, Name("n")]
        public int Nothing { get; set; }
    }
}
