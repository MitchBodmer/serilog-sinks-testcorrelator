using System;
using System.Collections.Generic;
using Serilog.Core;
using Serilog.Events;

namespace Serilog.Utilities.ConcurrentCorrelator
{
    public static class TestSerilogLogEvents
    {
        static readonly Logger TestLogger;

        static readonly TestCorrelationContextSink TestCorrelationContextSink = new TestCorrelationContextSink();

        static TestSerilogLogEvents()
        {
            TestLogger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Sink(TestCorrelationContextSink)
                .Enrich.FromLogContext()
                .CreateLogger();
        }

        public static void ConfigureGlobalLoggerForTesting()
        {
            Log.Logger = TestLogger;
        }

        public static ITestCorrelationContext CreateTestCorrelationContext()
        {
            ThrowIfGlobalLoggerIsNotConfiguredForTesting();

            return TestCorrelationContextSink.CreateTestCorrelationContext();
        }

        public static IEnumerable<LogEvent> GetLogEventsFromTestCorrelationContext(Guid testCorrelationContextGuid)
        {
            ThrowIfGlobalLoggerIsNotConfiguredForTesting();

            return TestCorrelationContextSink.GetLogEventsFromTestCorrelationContext(testCorrelationContextGuid);
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
    }
}