using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Serilog.Core;
using Serilog.Events;

namespace Serilog.Utilities.ConcurrentCorrelator
{
    public static class TestSerilogLogEvents
    {
        static readonly ConcurrentBag<LogEvent> Bag = new ConcurrentBag<LogEvent>();

        static readonly Logger TestLogger;

        static TestSerilogLogEvents()
        {
            TestLogger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.ConcurrentBag(Bag)
                .Enrich.FromLogContext()
                .CreateLogger();
        }

        public static void ConfigureGlobalLoggerForTesting()
        {
            Log.Logger = TestLogger;
        }

        public static IEnumerable<LogEvent> WithCorrelationLogContextGuid(Guid correlationLogContextGuid)
        {
            return Bag.Where(logEvent => logEvent.Properties.ContainsKey(correlationLogContextGuid.ToString()));
        }
    }
}