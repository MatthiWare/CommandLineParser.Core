using System;
using System.Threading;
using MatthiWare.CommandLine;
using MatthiWare.CommandLine.Abstractions;
using MatthiWare.CommandLine.Abstractions.Command;
using MatthiWare.CommandLine.Core.Attributes;
using Xunit;

namespace MatthiWare.CommandLineParser.Tests.Command
{
    public class SubCommandTests
    {
        [Fact]
        public void TestSubCommandWorksCorrectly()
        {
            var lock1 = new ManualResetEventSlim();
            var lock2 = new ManualResetEventSlim();

            var containerResolver = new CustomInstantiator(lock1, lock2);

            var parser = new CommandLineParser<MainModel>(containerResolver);

            var result = parser.Parse(new[] { "main", "-b", "something", "sub", "-i", "15" });

            Assert.False(result.HasErrors);

            Assert.True(lock1.Wait(1000), "MainCommand didn't execute in time.");
            Assert.True(lock2.Wait(1000), "SubCommand didn't execute in time.");
        }

        private class CustomInstantiator : IContainerResolver
        {
            private ManualResetEventSlim lock1;
            private ManualResetEventSlim lock2;

            public CustomInstantiator(ManualResetEventSlim lock1, ManualResetEventSlim lock2)
            {
                this.lock1 = lock1;
                this.lock2 = lock2;
            }

            public T Resolve<T>()
            {
                if (typeof(T) == typeof(MainCommand))
                    return (T)Activator.CreateInstance(typeof(T), lock1);
                else if (typeof(T) == typeof(SubCommand))
                    return (T)Activator.CreateInstance(typeof(T), lock2);
                else
                    return default;
            }
        }
    }

    public class MainCommand : Command<MainModel, SubModel>
    {
        private readonly ManualResetEventSlim locker;

        public MainCommand(ManualResetEventSlim locker)
        {
            this.locker = locker;
        }

        public override void OnConfigure(ICommandConfigurationBuilder builder)
        {
            builder
                .Name("main")
                .Required();
        }

        public override void OnExecute(MainModel options, SubModel commandOptions)
        {
            base.OnExecute(options, commandOptions);

            locker.Set();
        }
    }

    public class SubCommand : Command<MainModel, SubModel>
    {

        private readonly ManualResetEventSlim locker;

        public SubCommand(ManualResetEventSlim locker)
        {
            this.locker = locker;
        }

        public override void OnConfigure(ICommandConfigurationBuilder<SubModel> builder)
        {
            builder
                .Name("sub")
                .Required();
        }

        public override void OnExecute(MainModel options, SubModel commandOptions)
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
}
