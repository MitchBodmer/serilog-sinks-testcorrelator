using System;

namespace SerilogTestCorrelation
{
    /// <summary>
    /// A context used to capture and group LogEvents emitted by Serilog within a test.
    /// </summary>
    public interface ITestCorrelatorContext : IDisposable
    {
        /// <summary>
        /// Uniquely identifies a context. Can be passed to <seealso cref="TestCorrelator.GetLogEventsFromContext"/> to get the LogEvents emitted within the context.
        /// </summary>
        Guid Guid { get; }
    }
}