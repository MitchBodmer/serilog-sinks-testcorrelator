using System;

namespace Serilog.Utilities.ConcurrentCorrelator
{
    public static partial class TestSerilogLogEvents
    {
        public partial class TestLogContext
        {
            public class TestLogContextIdentifier
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