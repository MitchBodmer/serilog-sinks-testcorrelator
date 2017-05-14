using System;

namespace Serilog.Utilities.ConcurrentCorrelator
{
    class TestLogContextIdentifier : ITestLogContextIdentifier
    {
        public Guid Guid { get; }

        public TestLogContextIdentifier()
        {
            Guid = Guid.NewGuid();
        }
    }
}