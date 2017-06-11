using System;

namespace Serilog.Utilities.ConcurrentCorrelator
{
    class TestCorrelationContext : ITestCorrelationContext
    {
        public TestCorrelationContext()
        {
            Guid = Guid.NewGuid();
            LogicalCallContext.Add(Guid);
        }

        public Guid Guid { get; }

        public void Dispose()
        {
            LogicalCallContext.Remove(Guid);
        }
    }
}