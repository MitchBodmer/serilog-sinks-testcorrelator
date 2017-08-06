using Serilog.Core;
using Serilog.Events;

namespace Serilog.Sinks.TestCorrelator
{
    /// <summary>
    /// A sink to that writes to the <seealso cref="TestCorrelator"/>.
    /// </summary>
    public class TestCorrelatorSink : ILogEventSink
    {
        /// <summary>
        /// Emits the provided log event to the sink.
        /// </summary>
        /// <param name="logEvent">The log event to write.</param>
        public void Emit(LogEvent logEvent)
        {
            TestCorrelator.AddLogEvent(logEvent);
        }
    }
}