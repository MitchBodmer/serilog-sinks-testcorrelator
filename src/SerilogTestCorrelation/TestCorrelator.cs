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
    public static class TestCorrelator
    {
        static readonly Logger TestLogger;

        static readonly TestCorrelatorContextSink TestCorrelatorContextSink = new TestCorrelatorContextSink();

        static TestCorrelator()
        {
            TestLogger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Sink(TestCorrelatorContextSink)
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
        /// Creates a disposable <seealso cref="ITestCorrelatorContext"/> that captures all LogEvents emitted within it.
        /// </summary>
        /// <returns>The <seealso cref="ITestCorrelatorContext"/>.</returns>
        /// <exception cref="GlobalLoggerNotConfiguredForTestCorrelationException">Thrown when Serilog's global logger has not be configured for test correlation.</exception>
        public static ITestCorrelatorContext CreateTestCorrelationContext()
        {
            ThrowIfGlobalLoggerIsNotConfiguredForTesting();

            return TestCorrelatorContextSink.CreateTestCorrelationContext();
        }

        /// <summary>
        /// Gets the LogEvents emitted within an <seealso cref="ITestCorrelatorContext"/> with the provided GUID.
        /// </summary>
        /// <param name="testCorrelationContextGuid">The <seealso cref="ITestCorrelatorContext.Guid"/> of the desired context.</param>
        /// <returns>LogEvents emitted within the context.</returns>
        /// <exception cref="GlobalLoggerNotConfiguredForTestCorrelationException">Thrown when Serilog's global logger has not be configured for test correlation.</exception>
        public static IEnumerable<LogEvent> GetLogEventsFromTestCorrelationContext(Guid testCorrelationContextGuid)
        {
            ThrowIfGlobalLoggerIsNotConfiguredForTesting();

            return TestCorrelatorContextSink.GetLogEventsFromTestCorrelationContext(testCorrelationContextGuid);
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