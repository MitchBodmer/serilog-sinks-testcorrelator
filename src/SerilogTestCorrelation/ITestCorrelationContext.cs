using System;

namespace SerilogTestCorrelation
{
    public interface ITestCorrelationContext : IDisposable
    {
        Guid Guid { get; }
    }
}