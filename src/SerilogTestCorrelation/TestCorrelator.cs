using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Serilog.Events;

namespace SerilogTestCorrelation
{
    /// <summary>
    /// Correlates Serilog LogEvents to the test code that produced them.
    /// </summary>
    public static class TestCorrelator
    {
        static readonly ConcurrentDictionary<Guid, ConcurrentBag<LogEvent>> TestCorrelationContextGuidBags =
            new ConcurrentDictionary<Guid, ConcurrentBag<LogEvent>>();

        /// <summary>
        /// Creates a disposable <seealso cref="ITestCorrelatorContext"/> that captures all LogEvents emitted within it.
        /// </summary>
        /// <returns>The <seealso cref="ITestCorrelatorContext"/>.</returns>
        public static ITestCorrelatorContext CreateTestCorrelationContext()
        {
            var testCorrelationContext = new TestCorrelatorContext();

            TestCorrelationContextGuidBags.GetOrAdd(testCorrelationContext.Guid, new ConcurrentBag<LogEvent>());

            return testCorrelationContext;
        }

        /// <summary>
        /// Gets the LogEvents emitted within an <seealso cref="ITestCorrelatorContext"/> with the provided GUID.
        /// </summary>
        /// <param name="contextGuid">The <seealso cref="ITestCorrelatorContext.Guid"/> of the desired context.</param>
        /// <returns>LogEvents emitted within the context.</returns>
        public static IEnumerable<LogEvent> GetLogEventsFromTestCorrelationContext(Guid contextGuid)
        {
            return TestCorrelationContextGuidBags[contextGuid];
        }

        internal static void AddLogEvent(LogEvent logEvent)
        {
            foreach (var guid in TestCorrelationContextGuidBags.Keys)
            {
                if (LogicalCallContext.Contains(guid))
                {
                    TestCorrelationContextGuidBags[guid].Add(logEvent);
                }
            }
        }
    }
}