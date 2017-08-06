using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Serilog.Events;

namespace Serilog.Sinks.TestCorrelator
{
    /// <summary>
    /// Correlates Serilog LogEvents to the test code that produced them.
    /// </summary>
    public static class TestCorrelator
    {
        static readonly ConcurrentDictionary<Guid, ConcurrentBag<LogEvent>> TestCorrelatorContextGuidBags =
            new ConcurrentDictionary<Guid, ConcurrentBag<LogEvent>>();

        /// <summary>
        /// Creates a disposable <seealso cref="ITestCorrelatorContext"/> that captures all LogEvents emitted within it.
        /// </summary>
        /// <returns>The <seealso cref="ITestCorrelatorContext"/>.</returns>
        public static ITestCorrelatorContext CreateContext()
        {
            var testCorrelatorContext = new TestCorrelatorContext();

            TestCorrelatorContextGuidBags.GetOrAdd(testCorrelatorContext.Guid, new ConcurrentBag<LogEvent>());

            return testCorrelatorContext;
        }

        /// <summary>
        /// Gets the LogEvents emitted within an <seealso cref="ITestCorrelatorContext"/> with the provided GUID.
        /// </summary>
        /// <param name="contextGuid">The <seealso cref="ITestCorrelatorContext.Guid"/> of the desired context.</param>
        /// <returns>LogEvents emitted within the context.</returns>
        public static IEnumerable<LogEvent> GetLogEventsFromContext(Guid contextGuid)
        {
            return TestCorrelatorContextGuidBags[contextGuid];
        }

        internal static void AddLogEvent(LogEvent logEvent)
        {
            foreach (var guid in TestCorrelatorContextGuidBags.Keys)
            {
                if (LogicalCallContext.Contains(guid))
                {
                    TestCorrelatorContextGuidBags[guid].Add(logEvent);
                }
            }
        }
    }
}