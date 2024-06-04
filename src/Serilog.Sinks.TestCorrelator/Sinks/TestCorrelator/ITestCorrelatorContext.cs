using System;
using Serilog.Events;

namespace Serilog.Sinks.TestCorrelator;

/// <summary>
/// A context used to group <seealso cref="LogEvent"/>s emitted to the <seealso cref="TestCorrelator"/>.
/// </summary>
public interface ITestCorrelatorContext : IDisposable
{
    /// <summary>
    /// Uniquely identifies a context.
    /// </summary>
    TestCorrelatorContextId Id { get; }
}