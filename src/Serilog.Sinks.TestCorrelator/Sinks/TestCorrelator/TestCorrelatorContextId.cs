namespace Serilog.Sinks.TestCorrelator;

/// <summary>
/// Uniquely identifies a context. Can be passed to <seealso cref="TestCorrelator.GetLogEventsFromContextId"/> to get the LogEvents emitted within a specific context.
/// </summary>
public class TestCorrelatorContextId;