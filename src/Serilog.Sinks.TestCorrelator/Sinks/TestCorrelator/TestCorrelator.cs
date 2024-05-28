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
/// Correlates <seealso cref="LogEvent"/>s to the test code that produced them.
/// </summary>
public static class TestCorrelator
{
    private static readonly ConcurrentQueue<CapturedLogEvent> CapturedLogEventConcurrentQueue = new();

    private static readonly Subject<CapturedLogEvent> CapturedLogEventSubject = new();

    /// <summary>
    /// Creates an <seealso cref="ITestCorrelatorContext"/> that groups all <seealso cref="LogEvent"/>s emitted to a <seealso cref="TestCorrelatorSink"/> within it.
    /// </summary>
    /// <returns>The context.</returns>
    public static ITestCorrelatorContext CreateContext() => new TestCorrelatorContext();

    /// <summary>
    /// Gets the <seealso cref="LogEvent"/>s emitted to a <seealso cref="TestCorrelatorSink"/> within a context with the provided <seealso cref="TestCorrelatorContextId"/>.
    /// </summary>
    /// <param name="contextId">The <seealso cref="TestCorrelatorContextId"/> of the desired context.</param>
    /// <returns><seealso cref="LogEvent"/>s emitted within the context with the provided <seealso cref="TestCorrelatorContextId"/>.</returns>
    public static IReadOnlyList<LogEvent> GetLogEventsFromContextId(TestCorrelatorContextId contextId) =>
        CapturedLogEventConcurrentQueue
            .Where(capturedLogEvent => capturedLogEvent.ContextIds.Contains(contextId))
            .Select(capturedLogEvent => capturedLogEvent.LogEvent)
            .ToList()
            .AsReadOnly();

    /// <summary>
    /// Gets the <seealso cref="LogEvent"/>s emitted to a <seealso cref="TestCorrelatorSink"/> within the current context.
    /// </summary>
    /// <returns><seealso cref="LogEvent"/>s emitted within the current context.</returns>
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
    /// Gets the <seealso cref="LogEvent"/>s emitted to a <seealso cref="TestCorrelatorSink"/> within a context with the provided <seealso cref="TestCorrelatorContextId"/> by a sink with the provided <seealso cref="TestCorrelatorSinkId"/>.
    /// </summary>
    /// <param name="sinkId">The <seealso cref="TestCorrelatorSinkId"/> of the desired sink.</param>
    /// <param name="contextId">The <seealso cref="TestCorrelatorContextId"/> of the desired context.</param>
    /// <returns><seealso cref="LogEvent"/>s emitted within the context with the provided <seealso cref="TestCorrelatorContextId"/> by a sink with the provided <seealso cref="TestCorrelatorSinkId"/>.</returns>
    public static IReadOnlyList<LogEvent> GetLogEventsForSinksFromContextId(TestCorrelatorSinkId sinkId, TestCorrelatorContextId contextId) =>
        CapturedLogEventConcurrentQueue
            .Where(capturedLogEvent => capturedLogEvent.ContextIds.Contains(contextId) && capturedLogEvent.SinkIds.Contains(sinkId))
            .Select(capturedLogEvent => capturedLogEvent.LogEvent)
            .ToList()
            .AsReadOnly();

    /// <summary>
    /// Gets the <seealso cref="LogEvent"/>s emitted to a <seealso cref="TestCorrelatorSink"/> within the current context by a sink with the provided <seealso cref="TestCorrelatorSinkId"/>.
    /// </summary>
    /// <param name="sinkId">The <seealso cref="TestCorrelatorSinkId"/> of the desired sink.</param>
    /// <returns><seealso cref="LogEvent"/>s emitted within the current context by a sink with the provided <seealso cref="TestCorrelatorSinkId"/>.</returns>
    public static IReadOnlyList<LogEvent> GetLogEventsForSinksFromCurrentContext(TestCorrelatorSinkId sinkId)
    {
        var currentContextIds = TestCorrelatorContext.CurrentIds;

        return CapturedLogEventConcurrentQueue
            .Where(capturedLogEvent => currentContextIds.IsSubsetOf(capturedLogEvent.ContextIds) && capturedLogEvent.SinkIds.Contains(sinkId))
            .Select(capturedLogEvent => capturedLogEvent.LogEvent)
            .ToList()
            .AsReadOnly();
    }

    /// <summary>
    /// Gets an observable that emits <seealso cref="LogEvent"/>s as they are emitted to a <seealso cref="TestCorrelatorSink"/> within a context with the provided <seealso cref="TestCorrelatorContextId"/>.
    /// </summary>
    /// <param name="contextId">The <seealso cref="TestCorrelatorContextId"/> of the desired context.</param>
    /// <returns>The observable for the <seealso cref="LogEvent"/>s emitted within the context with the provided <seealso cref="TestCorrelatorContextId"/>.</returns>
    public static IObservable<LogEvent> GetLogEventStreamFromContextId(TestCorrelatorContextId contextId) =>
        CapturedLogEventSubject
            .Where(capturedLogEvent => capturedLogEvent.ContextIds.Contains(contextId))
            .Select(capturedLogEvent => capturedLogEvent.LogEvent);

    /// <summary>
    /// Gets an observable that emits <seealso cref="LogEvent"/>s as they are emitted to a <seealso cref="TestCorrelatorSink"/> within the current context.
    /// </summary>
    /// <returns>The observable for the <seealso cref="LogEvent"/>s emitted within the current context.</returns>
    public static IObservable<LogEvent> GetLogEventStreamFromCurrentContext()
    {
        var currentContextIds = TestCorrelatorContext.CurrentIds;

        return CapturedLogEventSubject
            .Where(capturedLogEvent => currentContextIds.IsSubsetOf(capturedLogEvent.ContextIds))
            .Select(capturedLogEvent => capturedLogEvent.LogEvent);
    }

    /// <summary>
    /// Gets an observable that emits <seealso cref="LogEvent"/>s as they are emitted to a <seealso cref="TestCorrelatorSink"/> within a context with the provided <seealso cref="TestCorrelatorContextId"/> by a sink with the provided <seealso cref="TestCorrelatorSinkId"/>.
    /// </summary>
    /// <param name="sinkId">The <seealso cref="TestCorrelatorSinkId"/> of the desired sink.</param>
    /// <param name="contextId">The <seealso cref="TestCorrelatorContextId"/> of the desired context.</param>
    /// <returns>The observable for the <seealso cref="LogEvent"/>s emitted within the context with the provided <seealso cref="TestCorrelatorContextId"/> by a sink with the provided <seealso cref="TestCorrelatorSinkId"/>.</returns>
    public static IObservable<LogEvent> GetLogEventStreamForSinksFromContextId(TestCorrelatorSinkId sinkId, TestCorrelatorContextId contextId) =>
        CapturedLogEventSubject
            .Where(capturedLogEvent => capturedLogEvent.ContextIds.Contains(contextId) && capturedLogEvent.SinkIds.Contains(sinkId))
            .Select(capturedLogEvent => capturedLogEvent.LogEvent);

    /// <summary>
    /// Gets an observable that emits <seealso cref="LogEvent"/>s as they are emitted to a <seealso cref="TestCorrelatorSink"/> within the current context by a sink with the provided <seealso cref="TestCorrelatorSinkId"/>.
    /// </summary>
    /// <param name="sinkId">The <seealso cref="TestCorrelatorSinkId"/> of the desired sink.</param>
    /// <returns>The observable for the <seealso cref="LogEvent"/>s emitted within the current context by a sink with the provided <seealso cref="TestCorrelatorSinkId"/>.</returns>
    public static IObservable<LogEvent> GetLogEventStreamForSinksFromCurrentContext(TestCorrelatorSinkId sinkId)
    {
        var currentContextIds = TestCorrelatorContext.CurrentIds;

        return CapturedLogEventSubject
            .Where(capturedLogEvent => currentContextIds.IsSubsetOf(capturedLogEvent.ContextIds) && capturedLogEvent.SinkIds.Contains(sinkId))
            .Select(capturedLogEvent => capturedLogEvent.LogEvent);
    }

    internal static void AddLogEvent(ImmutableHashSet<TestCorrelatorSinkId> sinkIds, LogEvent logEvent)
    {
        if (TestCorrelatorContext.CurrentIds.IsEmpty)
        {
            return;
        }

        var capturedLogEvent =
            new CapturedLogEvent(logEvent, sinkIds, TestCorrelatorContext.CurrentIds);

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

    private record CapturedLogEvent(LogEvent LogEvent, ImmutableHashSet<TestCorrelatorSinkId> SinkIds, ImmutableHashSet<TestCorrelatorContextId> ContextIds);
}