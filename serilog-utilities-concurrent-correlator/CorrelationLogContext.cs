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
            this.context = LogContext.PushProperty("CorrelationGuid", Guid);
        }

        public Guid Guid { get; }

        public void Dispose()
        {
            this.context.Dispose();
        }
    }
}