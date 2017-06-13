using System;
using System.Collections.Generic;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace SerilogTestCorrelation
{
    public static class SerilogTestCorrelator
    {
        static readonly Logger TestLogger;

        static readonly TestCorrelationContextSink TestCorrelationContextSink = new TestCorrelationContextSink();

        static SerilogTestCorrelator()
        {
            TestLogger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Sink(TestCorrelationContextSink)
                .Enrich.FromLogContext()
                .CreateLogger();
        }

        public static void ConfigureGlobalLoggerForTestCorrelation()
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
                throw new GlobalLoggerNotConfiguredForTestCorrelationException();
            }
        }

        static bool GlobalLoggerIsConfiguredForTesting()
        {
            return Log.Logger == TestLogger;
        }
    }
}