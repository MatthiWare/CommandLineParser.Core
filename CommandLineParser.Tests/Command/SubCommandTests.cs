using MatthiWare.CommandLine.Abstractions.Command;
using MatthiWare.CommandLine.Core.Attributes;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace MatthiWare.CommandLine.Tests.Command
{
    public class SubCommandTests  : TestBase
    {
        public SubCommandTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        [Theory]
        [InlineData(true, "something", 15, -1)]
        [InlineData(false, "something", 15, -1)]
        [InlineData(true, "", 15, -1)]
        public void TestSubCommandWorksCorrectlyInModel(bool autoExecute, string bla, int i, int n)
        {
            var lock1 = new ManualResetEventSlim();
            var lock2 = new ManualResetEventSlim();

            Services.AddSingleton(new MainCommand(lock1, autoExecute, bla, i, n));
            Services.AddSingleton(new SubCommand(lock2, autoExecute, bla, i, n));

            var parser = new CommandLineParser<MainModel>(Services);

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

        [Theory]
        [InlineData(true, "something", 15, -1)]
        [InlineData(false, "something", 15, -1)]
        [InlineData(true, "", 15, -1)]
        public async Task TestSubCommandWorksCorrectlyInModelAsync(bool autoExecute, string bla, int i, int n)
        {
            var lock1 = new ManualResetEventSlim();
            var lock2 = new ManualResetEventSlim();

            Services.AddSingleton(new MainCommand(lock1, autoExecute, bla, i, n));
            Services.AddSingleton(new SubCommand(lock2, autoExecute, bla, i, n));

            var parser = new CommandLineParser<MainModel>(Services);

            var result = await parser.ParseAsync(new[] { "main", "-b", bla, "sub", "-i", i.ToString(), "-n", n.ToString() });

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

            public override Task OnExecuteAsync(MainModel options, SubModel commandOptions, CancellationToken cancellationToken)
            {
                base.OnExecuteAsync(options, commandOptions, cancellationToken);

                Assert.Equal(bla, options.Bla);
                Assert.Equal(i, commandOptions.Item);

                locker.Set();

                return Task.CompletedTask;
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

            public override Task OnExecuteAsync(MainModel options, SubSubModel commandOptions, CancellationToken cancellationToken)
            {
                base.OnExecuteAsync(options, commandOptions, cancellationToken);

                Assert.Equal(bla, options.Bla);
                Assert.Equal(n, commandOptions.Nothing);

                locker.Set();

                return Task.CompletedTask;
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
