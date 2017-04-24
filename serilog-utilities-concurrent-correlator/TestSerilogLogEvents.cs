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

        public static TestLogContext EstablishTestLogContext()
        {
            ThrowIfGlobalLoggerIsNotConfiguredForTesting();

            return new TestLogContext();
        }

        public static IEnumerable<LogEvent> WithTestLogContextGuid(Guid correlationLogContextGuid)
        {
            ThrowIfGlobalLoggerIsNotConfiguredForTesting();

            return Bag.Where(logEvent => logEvent.Properties.ContainsKey(correlationLogContextGuid.ToString()));
        }

        static void ThrowIfGlobalLoggerIsNotConfiguredForTesting()
        {
            if (!GlobalLoggerIsConfiguredForTesting())
            {
                throw new TestSerilogEventsNotConfiguredException();
            }
        }

        static bool GlobalLoggerIsConfiguredForTesting()
        {
            return Log.Logger == TestLogger;
        }

        public class TestLogContext : IDisposable
        {
            readonly IDisposable context;

            internal TestLogContext()
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

        public class TestSerilogEventsNotConfiguredException : Exception
        {
            internal TestSerilogEventsNotConfiguredException() :
                base(
                    "The global logger has not been configured for testing. This can either be because you did not call TestSerilogEvents.ConfigureGlobalLoggerForTesting(), or because other code has overridden the global logger.")
            { }
        }
    }
}