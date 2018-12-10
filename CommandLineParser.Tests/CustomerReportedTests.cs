using System;
using System.Collections.Generic;
using System.Text;
using MatthiWare.CommandLine;
using Xunit;

namespace MatthiWare.CommandLineParser.Tests
{
    public class CustomerReportedTests
    {
        /// <summary>
        /// Running with *no* parameters at all crashes the command line parser #12
        /// https://github.com/MatthiWare/CommandLineParser.Core/issues/12
        /// </summary>
        [Theory]
        [InlineData(true, true)]
        [InlineData(false, false)]
        public void NoCommandLineArgumentsCrashesParser_Issue_12(bool required, bool outcome)
        {
            var parser = new CommandLineParser<OptionsModelIssue_12>();

            parser.Configure(opt => opt.Test)
                .Name("-1")
                .Default(1)
                .Required(required);

            var parsed = parser.Parse(new[] { "app.exe" });

            Assert.NotNull(parsed);

            Assert.Equal(outcome, parsed.HasErrors);
        }

        private class OptionsModelIssue_12
        {
            public int Test { get; set; }
        }
    }
}
