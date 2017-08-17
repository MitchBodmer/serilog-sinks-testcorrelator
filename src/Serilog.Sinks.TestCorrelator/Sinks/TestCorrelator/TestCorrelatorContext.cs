using System;

namespace Serilog.Sinks.TestCorrelator
{
    class TestCorrelatorContext : ITestCorrelatorContext
    {
        public TestCorrelatorContext()
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