using System;

namespace Serilog.Utilities.ConcurrentCorrelator
{
    public static partial class TestSerilogLogEvents
    {
        public partial class TestLogContext
        {
            public class TestLogContextIdentifier
            {
                Guid guid;

                internal TestLogContextIdentifier()
                {
                    guid = Guid.NewGuid();
                }

                public override string ToString()
                {
                    return guid.ToString();
                }
            }
        }
    }
}