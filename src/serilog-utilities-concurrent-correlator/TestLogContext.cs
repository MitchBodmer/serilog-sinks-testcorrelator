using System;

namespace Serilog.Utilities.ConcurrentCorrelator
{
    class TestLogContext : ITestLogContext
    {
        public TestLogContext()
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