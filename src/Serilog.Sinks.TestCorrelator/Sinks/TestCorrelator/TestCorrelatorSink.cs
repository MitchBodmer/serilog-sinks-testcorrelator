using Serilog.Core;
using Serilog.Events;

namespace Serilog.Sinks.TestCorrelator
{
    /// <summary>
    /// 
    /// </summary>
    public class TestCorrelatorSink : ILogEventSink
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logEvent"></param>
        public void Emit(LogEvent logEvent)
        {
            TestCorrelator.AddLogEvent(logEvent);
        }
    }
}