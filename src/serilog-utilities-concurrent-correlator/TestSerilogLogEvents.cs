using System;
using System.Collections.Generic;
using Serilog.Core;
using Serilog.Events;

namespace Serilog.Utilities.ConcurrentCorrelator
{
    public static class TestSerilogLogEvents
    {
        static readonly Logger TestLogger;

        static readonly TestLogContextSink TestLogContextSink = new TestLogContextSink();

        static TestSerilogLogEvents()
        {
            TestLogger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Sink(TestLogContextSink)
                .Enrich.FromLogContext()
                .CreateLogger();
        }

        public static void ConfigureGlobalLoggerForTesting()
        {
            Log.Logger = TestLogger;
        }

        public static ITestLogContext EstablishTestLogContext()
        {
            ThrowIfGlobalLoggerIsNotConfiguredForTesting();

            return TestLogContextSink.CreateTestLogContext();
        }

        public static IEnumerable<LogEvent> GetLogEventsWithContextIdentifier(Guid testLogContextIdentifier)
        {
            ThrowIfGlobalLoggerIsNotConfiguredForTesting();

            return TestLogContextSink.GetLogEventsFromTestLogContext(testLogContextIdentifier);
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