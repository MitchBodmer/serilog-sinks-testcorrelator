using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Serilog.Events;

namespace Serilog.Sinks.TestCorrelator
{
    /// <summary>
    /// Correlates Serilog LogEvents to the test code that produced them.
    /// </summary>
    public static class TestCorrelator
    {
        static readonly ConcurrentDictionary<LogEvent, ConcurrentBag<Guid>> LogEventGuidDictionary =
            new ConcurrentDictionary<LogEvent, ConcurrentBag<Guid>>();

        static readonly ConcurrentBag<Guid> TestCorrelationContextGuids = new ConcurrentBag<Guid>();

        /// <summary>
        /// Creates a disposable <seealso cref="ITestCorrelatorContext"/> that groups all LogEvents emitted to a <seealso cref="TestCorrelatorSink"/> within it.
        /// </summary>
        /// <returns>The <seealso cref="ITestCorrelatorContext"/>.</returns>
        public static ITestCorrelatorContext CreateContext()
        {
            var testCorrelatorContext = new TestCorrelatorContext();

            TestCorrelationContextGuids.Add(testCorrelatorContext.Guid);

            return testCorrelatorContext;
        }

        /// <summary>
        /// Gets the LogEvents emitted to a <seealso cref="TestCorrelatorSink"/> within an <seealso cref="ITestCorrelatorContext"/> with the provided GUID.
        /// </summary>
        /// <param name="contextGuid">The <seealso cref="ITestCorrelatorContext.Guid"/> of the desired context.</param>
        /// <returns>LogEvents emitted within the context.</returns>
        public static IEnumerable<LogEvent> GetLogEventsFromContextGuid(Guid contextGuid)
        {
           return LogEventGuidDictionary.Keys.Where(logEvent => LogEventGuidDictionary[logEvent].Contains(contextGuid));
        }

        internal static void AddLogEvent(LogEvent logEvent)
        {
            var guidBag = LogEventGuidDictionary.GetOrAdd(logEvent, new ConcurrentBag<Guid>());
            foreach (var guid in TestCorrelationContextGuids.Where(LogicalCallContext.Contains))
            {
                guidBag.Add(guid);
            }
        }
    }
}