using System;
using System.Collections.Generic;
using System.Text;

namespace MatthiWare.CommandLine.Abstractions.Usage
{
    public interface IUsageDisplay
    {
        string ToUsage();
        string ToShortUsage();
    }
}
