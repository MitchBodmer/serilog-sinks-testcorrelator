using System;

namespace Serilog.Utilities.ConcurrentCorrelator
{
    public interface ITestLogContextIdentifier
    {
        Guid Guid { get; }
    }
}