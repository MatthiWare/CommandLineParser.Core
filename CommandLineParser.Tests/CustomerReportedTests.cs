using System;
using System.Collections.Generic;
using MatthiWare.CommandLine;
using MatthiWare.CommandLine.Core.Attributes;
using Xunit;

namespace MatthiWare.CommandLine.Tests
{
    public class CustomerReportedTests
    {
        #region Issue_12
        /// <summary>
        /// Running with *no* parameters at all crashes the command line parser #12
        /// https://github.com/MatthiWare/CommandLineParser.Core/issues/12
        /// </summary>
        [Theory]
        [InlineData(true, true, false)]
        [InlineData(false, false, false)]
        [InlineData(true, true, true)]
        [InlineData(false, false, true)]
        public void NoCommandLineArgumentsCrashesParser_Issue_12(bool required, bool outcome, bool empty)
        {
            var parser = new CommandLineParser<OptionsModelIssue_12>();

            parser.Configure(opt => opt.Test)
                .Name("1")
                .Default(1)
                .Required(required);

            var parsed = parser.Parse(empty ? new string[] { } : new[] { "app.exe" });

            Assert.NotNull(parsed);

            Assert.Equal(outcome, parsed.HasErrors);
        }

        private class OptionsModelIssue_12
        {
            public int Test { get; set; }
        }
        #endregion

        #region Issue_30_AutoPrintUsageAndErrors
        /// <summary>
        /// AutoPrintUsageAndErrors always prints even when everything is *fine* #30
        /// https://github.com/MatthiWare/CommandLineParser.Core/issues/30
        /// </summary>
        [Theory]
        [InlineData(null, null)]
        [InlineData("true", null)]
        [InlineData("false", null)]
        [InlineData(null, "bla")]
        public void AutoPrintUsageAndErrorsShouldNotPrintWhenEverythingIsFIne(string verbose, string path)
        {
            var parser = new CommandLineParser<OptionsModelIssue_30>();

            var items = new List<string>();
            AddItemToArray(verbose);
            AddItemToArray(path);

            var parsed = parser.Parse(items.ToArray());

            if (parsed.HasErrors)
            {
                foreach (var err in parsed.Errors)
                    throw err;
            }

            void AddItemToArray(string item)
            {
                if (item != null)
                    items.Add(item);
            }
        }

        private class OptionsModelIssue_30
        {
            [Required]
            [Name("v", "verb")]
            [DefaultValue(true)]
            [Description("Verbose description")]
            public bool Verbose { get; set; }

            [Required]
            [Name("p", "path")]
            [DefaultValue(@"C:\Some\Path")]
            [Description("Path description")]
            public bool Path { get; set; }
        }
        #endregion
    }
}
