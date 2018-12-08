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
        static readonly ConcurrentQueue<ContextGuidDecoratedLogEvent> ContextGuidDecoratedLogEvents = new ConcurrentQueue<ContextGuidDecoratedLogEvent>();

        static readonly ConcurrentBag<Guid> ContextGuids = new ConcurrentBag<Guid>();

        /// <summary>
        /// Creates a disposable <seealso cref="ITestCorrelatorContext"/> that groups all LogEvents emitted to a <seealso cref="TestCorrelatorSink"/> within it.
        /// </summary>
        /// <returns>The <seealso cref="ITestCorrelatorContext"/>.</returns>
        public static ITestCorrelatorContext CreateContext()
        {
            var testCorrelatorContext = new TestCorrelatorContext();

            ContextGuids.Add(testCorrelatorContext.Guid);

            return testCorrelatorContext;
        }

        /// <summary>
        /// Gets the LogEvents emitted to a <seealso cref="TestCorrelatorSink"/> within an <seealso cref="ITestCorrelatorContext"/> with the provided GUID.
        /// </summary>
        /// <param name="contextGuid">The <seealso cref="ITestCorrelatorContext.Guid"/> of the desired context.</param>
        /// <returns>LogEvents emitted within the <seealso cref="ITestCorrelatorContext"/> with the provided GUID.</returns>
        public static IEnumerable<LogEvent> GetLogEventsFromContextGuid(Guid contextGuid)
        {
           return ContextGuidDecoratedLogEvents
                .Where(contextGuidDecoratedLogEvent => contextGuidDecoratedLogEvent.ContextGuids.Contains(contextGuid))
                .Select(contextGuidDecoratedLogEvent => contextGuidDecoratedLogEvent.LogEvent);
        }

        /// <summary>
        /// Gets the LogEvents emitted to a <seealso cref="TestCorrelatorSink"/> within the current <seealso cref="ITestCorrelatorContext"/>.
        /// </summary>
        /// <returns>LogEvents emitted within the current <seealso cref="ITestCorrelatorContext"/>.</returns>
        public static IEnumerable<LogEvent> GetLogEventsFromCurrentContext()
        {
            var currentContextGuids = ContextGuids.Where(LogicalCallContext.Contains);

            return ContextGuidDecoratedLogEvents
                .Where(contextGuidDecoratedLogEvent => currentContextGuids.All(currentContextGuid => contextGuidDecoratedLogEvent.ContextGuids.Contains(currentContextGuid)))
                .Select(contextGuidDecoratedLogEvent => contextGuidDecoratedLogEvent.LogEvent);
        }

        internal static void AddLogEvent(LogEvent logEvent)
        {
            ContextGuidDecoratedLogEvents
                .Enqueue(new ContextGuidDecoratedLogEvent(logEvent, ContextGuids.Where(LogicalCallContext.Contains).ToList()));
        }
    }
}