using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Serilog.Events;

namespace Serilog.Utilities.ConcurrentCorrelator
{
    public static class TestSerilogLogEvents
    {
        static readonly ConcurrentBag<LogEvent> Bag = new ConcurrentBag<LogEvent>();

        public static void Initialize()
        {
            Log.Logger =
                new LoggerConfiguration().MinimumLevel.Verbose()
                    .WriteTo.ConcurrentBag(Bag)
                    .Enrich.FromLogContext()
                    .CreateLogger();
        }

        public static IEnumerable<LogEvent> WithCorrelationLogContextGuid(Guid correlationLogContextGuid)
        {
            return Bag.Where(logEvent => logEvent.Properties.ContainsKey(correlationLogContextGuid.ToString()));
        }
    }
}