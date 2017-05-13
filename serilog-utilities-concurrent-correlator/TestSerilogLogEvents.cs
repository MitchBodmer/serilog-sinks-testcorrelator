using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Serilog.Core;
using Serilog.Events;

namespace Serilog.Utilities.ConcurrentCorrelator
{
    public static partial class TestSerilogLogEvents
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

        public static IEnumerable<LogEvent> GetLogEventsWithContextIdentifier(TestLogContext.TestLogContextIdentifier testLogContextIdentifier)
        {
            ThrowIfGlobalLoggerIsNotConfiguredForTesting();

            return Bag.Where(logEvent => logEvent.Properties.ContainsKey(testLogContextIdentifier.Guid.ToString()));
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