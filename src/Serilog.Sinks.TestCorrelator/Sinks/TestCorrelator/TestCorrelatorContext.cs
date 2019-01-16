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

        public Guid Guid { get; private set; }

        public void Dispose()
        {
            LogicalCallContext.Remove(Guid);
            Guid = Guid.Empty;
        }
    }

    class GlobalTestCorrelatorContext : ITestCorrelatorContext
    {
        public GlobalTestCorrelatorContext(Guid guid)
        {
            Guid = guid;
        }

        public Guid Guid { get; private set; }

        public void Dispose()
        {
            Guid = Guid.Empty;
        }
    }
}