using MatthiWare.CommandLine;
using MatthiWare.CommandLine.Abstractions;
using MatthiWare.CommandLine.Abstractions.Command;
using MatthiWare.CommandLine.Core;
using MatthiWare.CommandLine.Core.Attributes;
using System;
using System.Linq;
using System.Threading;
using Xunit;

namespace MatthiWare.CommandLine.Tests.Command
{
    public class SubCommandTests
    {
        [Theory]
        [InlineData(true, "something", 15, -1)]
        [InlineData(false, "something", 15, -1)]
        [InlineData(true, "", 15, -1)]
        public void TestSubCommandWorksCorrectlyInModel(bool autoExecute, string bla, int i, int n)
        {
            var lock1 = new ManualResetEventSlim();
            var lock2 = new ManualResetEventSlim();

            var containerResolver = new CustomInstantiator(lock1, lock2, autoExecute, bla, i, n);

            var parser = new CommandLineParser<MainModel>(containerResolver);

            var result = parser.Parse(new[] { "main", "-b", bla, "sub", "-i", i.ToString(), "-n", n.ToString() });

            result.AssertNoErrors();

            if (!autoExecute)
            {
                Assert.All(result.CommandResults.Select(r => r.Executed), Assert.False);

                result.ExecuteCommands();
            }

            Assert.True(lock1.Wait(1000), "MainCommand didn't execute in time.");
            Assert.True(lock2.Wait(1000), "SubCommand didn't execute in time.");

            Assert.All(result.CommandResults.Select(r => r.Executed), Assert.True);
        }

        private class CustomInstantiator : DefaultContainerResolver
        {
            private readonly ManualResetEventSlim lock1;
            private readonly ManualResetEventSlim lock2;
            private readonly bool autoExecute;
            private readonly string bla;
            private readonly int i;
            private readonly int n;

            public CustomInstantiator(ManualResetEventSlim lock1, ManualResetEventSlim lock2, bool autoExecute, string bla, int i, int n)
            {
                this.lock1 = lock1;
                this.lock2 = lock2;
                this.autoExecute = autoExecute;
                this.bla = bla;
                this.i = i;
                this.n = n;
            }

            public override T Resolve<T>() => (T)Resolve(typeof(T));

            public override object Resolve(Type type)
            {
                if (type == typeof(MainCommand))
                    return Activator.CreateInstance(type, lock1, autoExecute, bla, i, n);
                else if (type == typeof(SubCommand))
                    return Activator.CreateInstance(type, lock2, autoExecute, bla, i, n);
                else
                    return base.Resolve(type);
            }
        }

        public class MainCommand : Command<MainModel, SubModel>
        {
            private readonly ManualResetEventSlim locker;
            private readonly bool autoExecute;
            private readonly string bla;
            private readonly int i;
            private readonly int n;

            public MainCommand(ManualResetEventSlim locker, bool autoExecute, string bla, int i, int n)
            {
                this.locker = locker;
                this.autoExecute = autoExecute;
                this.bla = bla;
                this.i = i;
                this.n = n;
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

                Assert.Equal(bla, options.Bla);
                Assert.Equal(i, commandOptions.Item);

                locker.Set();
            }
        }

        public class SubCommand : Command<MainModel, SubSubModel>
        {
            private readonly ManualResetEventSlim locker;
            private readonly bool autoExecute;
            private readonly string bla;
            private readonly int i;
            private readonly int n;

            public SubCommand(ManualResetEventSlim locker, bool autoExecute, string bla, int i, int n)
            {
                this.locker = locker;
                this.autoExecute = autoExecute;
                this.bla = bla;
                this.i = i;
                this.n = n;
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

                Assert.Equal(bla, options.Bla);
                Assert.Equal(n, commandOptions.Nothing);

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
}
