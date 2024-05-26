using System;

namespace Serilog.Sinks.TestCorrelator;

/// <summary>
/// A disposable used to group LogEvents emitted to the <seealso cref="TestCorrelator"/>.
/// </summary>
public interface ITestCorrelatorContext : IDisposable
{
    /// <summary>
    /// Uniquely identifies a context. Can be passed to <seealso cref="TestCorrelator.GetLogEventsFromContextGuid"/> to get the LogEvents emitted within the context.
    /// </summary>
    Guid Guid { get; }
}