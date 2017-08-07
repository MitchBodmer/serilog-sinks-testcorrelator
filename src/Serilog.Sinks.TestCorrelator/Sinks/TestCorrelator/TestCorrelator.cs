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

        static readonly ConcurrentBag<Guid> TestCorrelatorContextGuids = new ConcurrentBag<Guid>();

        /// <summary>
        /// Creates a disposable <seealso cref="ITestCorrelatorContext"/> that groups all LogEvents emitted to a <seealso cref="TestCorrelatorSink"/> within it.
        /// </summary>
        /// <returns>The <seealso cref="ITestCorrelatorContext"/>.</returns>
        public static ITestCorrelatorContext CreateContext()
        {
            var testCorrelatorContext = new TestCorrelatorContext();

            TestCorrelatorContextGuids.Add(testCorrelatorContext.Guid);

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

        /// <summary>
        /// Gets the LogEvents emitted within the current <seealso cref="ITestCorrelatorContext"/>.
        /// </summary>
        /// <returns>LogEvents emitted within the current <seealso cref="ITestCorrelatorContext"/>.</returns>
        public static IEnumerable<LogEvent> GetLogEventsFromCurrentContext()
        {
            var currentContextGuids = TestCorrelatorContextGuids.Where(LogicalCallContext.Contains);

            return LogEventGuidDictionary.Keys.Where(
                logEvent => currentContextGuids.All(
                    currentContextGuid => LogEventGuidDictionary[logEvent].Contains(currentContextGuid)));
        }

        internal static void AddLogEvent(LogEvent logEvent)
        {
            var guidBag = LogEventGuidDictionary.GetOrAdd(logEvent, new ConcurrentBag<Guid>());
            foreach (var guid in TestCorrelatorContextGuids.Where(LogicalCallContext.Contains))
            {
                guidBag.Add(guid);
            }
        }
    }
}