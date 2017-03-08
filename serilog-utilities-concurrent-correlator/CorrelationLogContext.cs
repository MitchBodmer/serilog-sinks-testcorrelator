using System;
using Serilog.Context;

namespace Serilog.Utilities.ConcurrentCorrelator
{
    public class CorrelationLogContext : IDisposable
    {
        readonly IDisposable context;

        public CorrelationLogContext()
        {
            Id = Guid.NewGuid();
            this.context = LogContext.PushProperty("CorrelationId", Id);
        }

        public Guid Id { get; }

        public void Dispose()
        {
            this.context.Dispose();
        }
    }
}