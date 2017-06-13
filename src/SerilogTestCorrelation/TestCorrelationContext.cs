using System;

namespace SerilogTestCorrelation
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