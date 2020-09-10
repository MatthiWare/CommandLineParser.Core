using MatthiWare.CommandLine.Abstractions.Command;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace MatthiWare.CommandLine.Tests
{
    public class BasicDITests : TestBase
    {
        public BasicDITests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        [Fact]
        public void CommandLineParserUsesInjectedServiceCorrectly()
        {
            var mockedService = new Mock<MySerice>();

            mockedService
                .Setup(_ => _.Call())
                .Verifiable();

            Services.AddSingleton(mockedService.Object);

            var parser = new CommandLineParser(Services);

            parser.RegisterCommand<MyCommandThatUsesService>();

            var result = parser.Parse(new[] { "app.exe", "cmd" });

            result.AssertNoErrors();

            mockedService.Verify(_ => _.Call(), Times.Once());
        }

        [Fact]
        public void CommandLineParserServiceResolvesCorrectly()
        {
            var mockedService = Mock.Of<MySerice>();

            Services.AddSingleton(mockedService);

            var parser = new CommandLineParser(Services);

            var resolved = parser.Services.GetRequiredService<MySerice>();

            Assert.Equal(mockedService, resolved);
        }
    }

    public interface MySerice
    {
        void Call();
    }

    public class MyCommandThatUsesService : Command<object>
    {
        private readonly MySerice serice;

        public MyCommandThatUsesService(MySerice serice)
        {
            this.serice = serice ?? throw new System.ArgumentNullException(nameof(serice));
        }

        public override void OnConfigure(ICommandConfigurationBuilder builder)
        {
            builder
                .Name("cmd")
                .AutoExecute(true)
                .Required(true);
        }

        public override void OnExecute()
        {
            base.OnExecute();

            serice.Call();
        }
    }
}
