using System;

namespace Serilog.Utilities.ConcurrentCorrelator
{
    public static partial class TestSerilogLogEvents
    {
        public class TestSerilogEventsNotConfiguredException : Exception
        {
            internal TestSerilogEventsNotConfiguredException() :
                base(
                    "The global logger has not been configured for testing. This can either be because you did not call TestSerilogEvents.ConfigureGlobalLoggerForTesting(), or because other code has overridden the global logger.")
            { }
        }
    }
}