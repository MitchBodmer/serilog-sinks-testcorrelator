using System;
using Serilog.Context;

namespace Serilog.Utilities.ConcurrentCorrelator
{
    public static partial class TestSerilogLogEvents
    {
        public class TestLogContext : IDisposable
        {
            readonly IDisposable context;

            public TestLogContext()
            {
                Identifier = new TestLogContextIdentifier();
                context = LogContext.PushProperty(Identifier.Guid.ToString(), null);
            }

            public ITestLogContextIdentifier Identifier { get; }

            public void Dispose()
            {
                context.Dispose();
            }
        }
    }
}