using System;

namespace Serilog.Utilities.ConcurrentCorrelator
{
    public static partial class TestSerilogLogEvents
    {
        public partial class TestLogContext
        {
            class TestLogContextIdentifier : ITestLogContextIdentifier
            {
                public Guid Guid { get; }

                internal TestLogContextIdentifier()
                {
                    Guid = Guid.NewGuid();
                }
            }
        }
    }
}