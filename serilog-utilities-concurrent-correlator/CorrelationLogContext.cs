using System;
using Serilog.Context;

namespace Serilog.Utilities.ConcurrentCorrelator
{
    public class CorrelationLogContext : IDisposable
    {
        readonly IDisposable context;

        public CorrelationLogContext()
        {
            Guid = Guid.NewGuid();
            this.context = LogContext.PushProperty(Guid.ToString(), null);
        }

        public Guid Guid { get; }

        public void Dispose()
        {
            this.context.Dispose();
        }
    }
}