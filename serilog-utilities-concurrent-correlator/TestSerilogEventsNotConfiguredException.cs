using System;

namespace Serilog.Utilities.ConcurrentCorrelator
{
    public class TestSerilogEventsNotConfiguredException : Exception
    {
        internal TestSerilogEventsNotConfiguredException() :
            base(
                $"The global logger has not been configured for testing. This can either be because you did not call {nameof(TestSerilogLogEvents.ConfigureGlobalLoggerForTesting)}, or because other code has overridden the global logger.")
        { }
    }
}