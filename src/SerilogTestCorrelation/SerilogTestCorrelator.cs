using System;
using System.Collections.Generic;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace SerilogTestCorrelation
{
    /// <summary>
    /// Correlates Serilog LogEvents to the test code that produced them.
    /// </summary>
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

        /// <summary>
        /// Configures Serilog's global logger (Serilog.Log.Logger) for test correlation.
        /// </summary>
        public static void ConfigureGlobalLoggerForTestCorrelation()
        {
            Log.Logger = TestLogger;
        }

        /// <summary>
        /// Creates a disposable <seealso cref="ITestCorrelationContext"/> that captures all LogEvents emitted within it.
        /// </summary>
        /// <returns>The <seealso cref="ITestCorrelationContext"/>.</returns>
        /// <exception cref="GlobalLoggerNotConfiguredForTestCorrelationException">Thrown when Serilog's global logger has not be configured for test correlation.</exception>
        public static ITestCorrelationContext CreateTestCorrelationContext()
        {
            ThrowIfGlobalLoggerIsNotConfiguredForTesting();

            return TestCorrelationContextSink.CreateTestCorrelationContext();
        }

        /// <summary>
        /// Gets the LogEvents emitted within an <seealso cref="ITestCorrelationContext"/> with the provided GUID.
        /// </summary>
        /// <param name="testCorrelationContextGuid">The <seealso cref="ITestCorrelationContext.Guid"/> of the desired context.</param>
        /// <returns>LogEvents emitted within the context.</returns>
        /// <exception cref="GlobalLoggerNotConfiguredForTestCorrelationException">Thrown when Serilog's global logger has not be configured for test correlation.</exception>
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