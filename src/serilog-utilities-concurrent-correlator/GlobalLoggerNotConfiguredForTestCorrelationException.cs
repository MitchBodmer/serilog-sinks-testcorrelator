using System;

namespace Serilog.Utilities.ConcurrentCorrelator
{
    public class GlobalLoggerNotConfiguredForTestCorrelationException : Exception
    {
        public GlobalLoggerNotConfiguredForTestCorrelationException() :
            base(
                $"Serilog's global logger has not been configured for test correlation. This can either be because you did not call {nameof(TestSerilogLogEvents.ConfigureGlobalLoggerForTestCorrelation)}, or because other code has overwritten {nameof(Log.Logger)} since you did.")
        { }
    }
}