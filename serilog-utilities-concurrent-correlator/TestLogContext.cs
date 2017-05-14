using System;
using Serilog.Context;

namespace Serilog.Utilities.ConcurrentCorrelator
{
    public class TestLogContext : IDisposable
    {
        readonly IDisposable context;

        public TestLogContext()
        {
            Guid = Guid.NewGuid();
            context = LogContext.PushProperty(Guid.ToString(), null);
        }

        public Guid Guid { get; }

        public void Dispose()
        {
            context.Dispose();
        }
    }
}