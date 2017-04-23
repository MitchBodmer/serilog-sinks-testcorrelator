using System.Collections.Concurrent;
using Serilog.Core;
using Serilog.Events;

namespace Serilog.Utilities.ConcurrentCorrelator
{
    class ConcurrentBagSink : ILogEventSink
    {
        readonly ConcurrentBag<LogEvent> concurrentBag;

        internal ConcurrentBagSink(ConcurrentBag<LogEvent> concurrentBag)
        {
            this.concurrentBag = concurrentBag;
        }

        public void Emit(LogEvent logEvent)
        {
            concurrentBag.Add(logEvent);
        }
    }
}