using System;
using Serilog.Context;

namespace Serilog.Utilities.ConcurrentCorrelator
{
    public static partial class TestSerilogLogEvents
    {
        partial class TestLogContext : IDisposable
        {
            readonly IDisposable context;

            public TestLogContext()
            {
                Identifier = new TestLogContextIdentifier();
                context = LogContext.PushProperty(Identifier.Guid.ToString(), null);
            }

            public TestLogContextIdentifier Identifier { get; }

            public void Dispose()
            {
                context.Dispose();
            }
        }
    }
}