using Serilog.Core;
using Serilog.Events;

namespace SerilogTestCorrelation
{
    public class TestCorrelatorSink : ILogEventSink
    {
        public void Emit(LogEvent logEvent)
        {
            TestCorrelator.AddLogEvent(logEvent);
        }
    }
}