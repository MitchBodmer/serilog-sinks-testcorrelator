using System;

namespace SerilogTestCorrelation
{
    /// <summary>
    /// A disposable used to capture LogEvents emitted to the <seealso cref="TestCorrelator"/>.
    /// </summary>
    public interface ITestCorrelatorContext : IDisposable
    {
        /// <summary>
        /// Uniquely identifies a context. Can be passed to <seealso cref="TestCorrelator.GetLogEventsFromContext"/> to get the LogEvents emitted within the context.
        /// </summary>
        Guid Guid { get; }
    }
}