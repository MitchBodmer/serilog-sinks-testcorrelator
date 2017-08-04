using Serilog.Core;
using Serilog.Events;

namespace Serilog.Sinks.TestCorrelator
{
    public class TestCorrelatorSink : ILogEventSink
    {
        public void Emit(LogEvent logEvent)
        {
            TestCorrelator.AddLogEvent(logEvent);
        }
    }
}