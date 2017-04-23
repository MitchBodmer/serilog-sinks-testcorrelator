using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Serilog.Context;
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

        public static CorrelationLogContext EstablishContext()
        {
            if (!GlobalLoggerIsConfiguredForTesting())
            {
                throw new Exception();
            }

            return new CorrelationLogContext();
        }

        public static IEnumerable<LogEvent> WithCorrelationLogContextGuid(Guid correlationLogContextGuid)
        {
            if (!GlobalLoggerIsConfiguredForTesting())
            {
                throw new Exception();
            }
            return Bag.Where(logEvent => logEvent.Properties.ContainsKey(correlationLogContextGuid.ToString()));
        }

        static bool GlobalLoggerIsConfiguredForTesting()
        {
            return Log.Logger == TestLogger;
        }

        public class CorrelationLogContext : IDisposable
        {
            readonly IDisposable context;

            internal CorrelationLogContext()
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
}