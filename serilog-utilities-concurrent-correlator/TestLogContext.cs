using System;
using Serilog.Context;

namespace Serilog.Utilities.ConcurrentCorrelator
{
    public static partial class TestSerilogLogEvents
    {
        public partial class TestLogContext : IDisposable
        {
            readonly IDisposable context;

            internal TestLogContext()
            {
                Identifier = new TestLogContextIdentifier();
                context = LogContext.PushProperty(Identifier.ToString(), null);
            }

            public TestLogContextIdentifier Identifier { get; }

            public void Dispose()
            {
                context.Dispose();
            }
        }
    }
}