using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using Serilog.Events;

namespace Serilog.Sinks.TestCorrelator;

/// <summary>
/// Correlates Serilog LogEvents to the test code that produced them.
/// </summary>
public static class TestCorrelator
{
    private static readonly ConcurrentQueue<CapturedLogEvent> CapturedLogEventConcurrentQueue = new();

    private static readonly Subject<CapturedLogEvent> CapturedLogEventSubject = new();

    /// <summary>
    /// Creates a disposable <seealso cref="ITestCorrelatorContext"/> that groups all LogEvents emitted to a <seealso cref="TestCorrelatorSink"/> within it.
    /// </summary>
    /// <returns>The <seealso cref="ITestCorrelatorContext"/>.</returns>
    public static ITestCorrelatorContext CreateContext() => new TestCorrelatorContext();

    /// <summary>
    /// Gets the LogEvents emitted to a <seealso cref="TestCorrelatorSink"/> within an <seealso cref="ITestCorrelatorContext"/> with the provided <seealso cref="TestCorrelatorContextId"/>.
    /// </summary>
    /// <param name="contextId">The <seealso cref="ITestCorrelatorContext.Id"/> of the desired context.</param>
    /// <returns>LogEvents emitted within the <seealso cref="ITestCorrelatorContext"/> with the provided <seealso cref="TestCorrelatorContextId"/>.</returns>
    public static IReadOnlyList<LogEvent> GetLogEventsFromContextId(TestCorrelatorContextId contextId) =>
        CapturedLogEventConcurrentQueue
            .Where(capturedLogEvent => capturedLogEvent.ContextIds.Contains(contextId))
            .Select(capturedLogEvent => capturedLogEvent.LogEvent)
            .ToList()
            .AsReadOnly();

    /// <summary>
    /// Gets the LogEvents emitted to a <seealso cref="TestCorrelatorSink"/> within the current <seealso cref="ITestCorrelatorContext"/>.
    /// </summary>
    /// <returns>LogEvents emitted within the current <seealso cref="ITestCorrelatorContext"/>.</returns>
    public static IReadOnlyList<LogEvent> GetLogEventsFromCurrentContext()
    {
        var currentContextIds = TestCorrelatorContext.CurrentIds;

        return CapturedLogEventConcurrentQueue
            .Where(capturedLogEvent => currentContextIds.IsSubsetOf(capturedLogEvent.ContextIds))
            .Select(capturedLogEvent => capturedLogEvent.LogEvent)
            .ToList()
            .AsReadOnly();
    }

    /// <summary>
    /// Gets an observable that emits LogEvents as they are emitted to a <seealso cref="TestCorrelatorSink"/> within an <seealso cref="ITestCorrelatorContext"/> with the provided <seealso cref="TestCorrelatorContextId"/>.
    /// </summary>
    /// <param name="contextId">The <seealso cref="ITestCorrelatorContext.Id"/> of the desired context.</param>
    /// <returns>The observable for the LogEvents emitted within the <seealso cref="ITestCorrelatorContext"/> with the provided <seealso cref="TestCorrelatorContextId"/>.</returns>
    public static IObservable<LogEvent> GetLogEventStreamFromcontextId(TestCorrelatorContextId contextId) =>
        CapturedLogEventSubject
            .Where(capturedLogEvent => capturedLogEvent.ContextIds.Contains(contextId))
            .Select(capturedLogEvent => capturedLogEvent.LogEvent);

    /// <summary>
    /// Gets an observable that emits LogEvents as they are emitted to a <seealso cref="TestCorrelatorSink"/> within the current <seealso cref="ITestCorrelatorContext"/>.
    /// </summary>
    /// <returns>The observable for the LogEvents emitted within the current <seealso cref="ITestCorrelatorContext"/>.</returns>
    public static IObservable<LogEvent> GetLogEventStreamFromCurrentContext()
    {
        var currentContextIds = TestCorrelatorContext.CurrentIds;

        return CapturedLogEventSubject
            .Where(capturedLogEvent => currentContextIds.IsSubsetOf(capturedLogEvent.ContextIds))
            .Select(capturedLogEvent => capturedLogEvent.LogEvent);
    }

    internal static void AddLogEvent(LogEvent logEvent)
    {
        if (TestCorrelatorContext.CurrentIds.IsEmpty)
        {
            return;
        }

        var capturedLogEvent =
            new CapturedLogEvent(logEvent, TestCorrelatorContext.CurrentIds);

        CapturedLogEventConcurrentQueue.Enqueue(capturedLogEvent);

        CapturedLogEventSubject.OnNext(capturedLogEvent);
    }

    private record TestCorrelatorContext : ITestCorrelatorContext
    {
        private static readonly AsyncLocal<ImmutableHashSet<TestCorrelatorContextId>> AsyncLocalIds = new();

        public TestCorrelatorContext()
        {
            Id = new TestCorrelatorContextId();
            AsyncLocalIds.Value = GetOrCreateTestCorrelatorContextIdSet().Add(Id);
        }

        public TestCorrelatorContextId Id { get; }

        public void Dispose() =>
            AsyncLocalIds.Value = GetOrCreateTestCorrelatorContextIdSet().Remove(Id);

        public static ImmutableHashSet<TestCorrelatorContextId> CurrentIds => GetOrCreateTestCorrelatorContextIdSet();

        private static ImmutableHashSet<TestCorrelatorContextId> GetOrCreateTestCorrelatorContextIdSet() =>
            AsyncLocalIds.Value ?? [];
    }

    private record CapturedLogEvent(LogEvent LogEvent, ImmutableHashSet<TestCorrelatorContextId> ContextIds);
}