using System;

namespace Serilog.Utilities.ConcurrentCorrelator
{
    public interface ITestLogContext : IDisposable
    {
        Guid Guid { get; }
    }
}