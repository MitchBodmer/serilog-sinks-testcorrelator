﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Serilog.Events;

namespace Serilog.Sinks.TestCorrelator;

/// <summary>
/// Correlates Serilog LogEvents to the test code that produced them.
/// </summary>
public static class TestCorrelator
{
    static readonly ConcurrentQueue<ContextGuidDecoratedLogEvent> ContextGuidDecoratedLogEvents = new();

    static readonly ConcurrentBag<Guid> ContextGuids = [];

    static readonly Subject<ContextGuidDecoratedLogEvent> ContextGuidDecoratedLogEventSubject = new();

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
        var currentContextGuids = GetCurrentContextGuids().ToList();

        return ContextGuidDecoratedLogEvents
            .Where(contextGuidDecoratedLogEvent => !currentContextGuids.Except(contextGuidDecoratedLogEvent.ContextGuids).Any())
            .Select(contextGuidDecoratedLogEvent => contextGuidDecoratedLogEvent.LogEvent);
    }

    /// <summary>
    /// Gets an observable that emits LogEvents as they are emitted to a <seealso cref="TestCorrelatorSink"/> within an <seealso cref="ITestCorrelatorContext"/> with the provided GUID.
    /// </summary>
    /// <param name="contextGuid">The <seealso cref="ITestCorrelatorContext.Guid"/> of the desired context.</param>
    /// <returns>The observable for the LogEvents emitted within the <seealso cref="ITestCorrelatorContext"/> with the provided GUID.</returns>
    public static IObservable<LogEvent> GetLogEventStreamFromContextGuid(Guid contextGuid)
    {
        return ContextGuidDecoratedLogEventSubject
            .Where(contextGuidDecoratedLogEvent => contextGuidDecoratedLogEvent.ContextGuids.Contains(contextGuid))
            .Select(contextGuidDecoratedLogEvent => contextGuidDecoratedLogEvent.LogEvent);
    }

    /// <summary>
    /// Gets an observable that emits LogEvents as they are emitted to a <seealso cref="TestCorrelatorSink"/> within the current <seealso cref="ITestCorrelatorContext"/>.
    /// </summary>
    /// <returns>The observable for the LogEvents emitted within the current <seealso cref="ITestCorrelatorContext"/>.</returns>
    public static IObservable<LogEvent> GetLogEventStreamFromCurrentContext()
    {
        var currentContextGuids = GetCurrentContextGuids().ToList();

        return ContextGuidDecoratedLogEventSubject
            .Where(contextGuidDecoratedLogEvent => !currentContextGuids.Except(contextGuidDecoratedLogEvent.ContextGuids).Any())
            .Select(contextGuidDecoratedLogEvent => contextGuidDecoratedLogEvent.LogEvent);
    }

    private static IEnumerable<Guid> GetCurrentContextGuids()
    {
        return ContextGuids.Where(LogicalCallContext.Contains);
    }

    internal static void AddLogEvent(LogEvent logEvent)
    {
        var contextGuidDecoratedLogEvent = new ContextGuidDecoratedLogEvent(logEvent, ContextGuids.Where(LogicalCallContext.Contains).ToList());

        ContextGuidDecoratedLogEvents.Enqueue(contextGuidDecoratedLogEvent);

        ContextGuidDecoratedLogEventSubject.OnNext(contextGuidDecoratedLogEvent);
    }
}