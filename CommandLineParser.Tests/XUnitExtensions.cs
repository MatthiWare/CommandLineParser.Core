using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Xunit.Sdk;

namespace MatthiWare.CommandLineParser.Tests
{
    public static class XUnitExtensions
    {


        public static void Fail(string reason)
            => throw new XunitException(reason);


    }
}
