using System.Collections.Generic;
using System.Collections.Immutable;
using Serilog.Core;
using Serilog.Events;

namespace Serilog.Sinks.TestCorrelator;

/// <summary>
/// A sink to that writes to the <seealso cref="TestCorrelator"/>.
/// </summary>
public class TestCorrelatorSink : ILogEventSink
{
    private readonly ImmutableHashSet<TestCorrelatorSinkId> _ids;

    /// <summary>
    /// A sink to that writes to the <seealso cref="TestCorrelator"/>.
    /// </summary>
    /// <param name="ids">The <seealso cref="TestCorrelatorSinkId"/>s to apply to this sink.</param>
    public TestCorrelatorSink(params TestCorrelatorSinkId[] ids) => _ids = [.. ids];

    /// <summary>
    /// A sink to that writes to the <seealso cref="TestCorrelator"/>.
    /// </summary>
    /// <param name="ids">The <seealso cref="TestCorrelatorSinkId"/>s to apply to this sink.</param>
    public TestCorrelatorSink(IEnumerable<TestCorrelatorSinkId> ids) => _ids = [.. ids];

    /// <summary>
    /// Emits the provided log event to the sink.
    /// </summary>
    /// <param name="logEvent">The log event to write.</param>
    public void Emit(LogEvent logEvent) => TestCorrelator.AddLogEvent(_ids, logEvent);
}
