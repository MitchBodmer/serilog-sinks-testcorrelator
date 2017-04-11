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
        static readonly ConcurrentBag<LogEvent> bag = new ConcurrentBag<LogEvent>();

        static readonly Logger testLogger;

        static TestSerilogLogEvents()
        {
            testLogger = new LoggerConfiguration().MinimumLevel.Verbose()
                .WriteTo.ConcurrentBag(bag)
                .Enrich.FromLogContext()
                .CreateLogger();
        }

        public static void ConfigureGlobalLoggerForTesting()
        {
            Log.Logger = testLogger;
        }

        public static IEnumerable<LogEvent> WithCorrelationLogContextGuid(Guid correlationLogContextGuid)
        {
            return bag.Where(logEvent => logEvent.Properties.ContainsKey(correlationLogContextGuid.ToString()));
        }
    }
}