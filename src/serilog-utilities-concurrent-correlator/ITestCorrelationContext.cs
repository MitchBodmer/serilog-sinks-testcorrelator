using System;

namespace Serilog.Utilities.ConcurrentCorrelator
{
    public interface ITestCorrelationContext : IDisposable
    {
        Guid Guid { get; }
    }
}