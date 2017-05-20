using System;

namespace Serilog.Utilities.ConcurrentCorrelator
{
    public class TestSerilogEventsNotConfiguredException : Exception
    {
        public TestSerilogEventsNotConfiguredException() :
            base(
                $"Serilog's global logger has not been configured for testing. This can either be because you did not call {nameof(TestSerilogLogEvents.ConfigureGlobalLoggerForTesting)}, or because other code has overwritten {nameof(Log.Logger)} since you did.")
        { }
    }
}