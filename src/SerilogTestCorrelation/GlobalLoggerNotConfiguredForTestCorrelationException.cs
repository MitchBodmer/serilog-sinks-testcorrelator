using System;
using Serilog;

namespace SerilogTestCorrelation
{
    public class GlobalLoggerNotConfiguredForTestCorrelationException : Exception
    {
        public GlobalLoggerNotConfiguredForTestCorrelationException() :
            base(
                $"Serilog's global logger has not been configured for test correlation. This can either be because you did not call {nameof(SerilogTestCorrelator.ConfigureGlobalLoggerForTestCorrelation)}, or because other code has overwritten {nameof(Log.Logger)} since you did.")
        {
        }
    }
}