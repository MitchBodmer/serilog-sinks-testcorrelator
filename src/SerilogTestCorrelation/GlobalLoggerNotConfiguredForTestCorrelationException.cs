using System;
using Serilog;

namespace SerilogTestCorrelation
{
    public class GlobalLoggerNotConfiguredForTestCorrelationException : Exception
    {
        public GlobalLoggerNotConfiguredForTestCorrelationException() :
            base(
                $"Serilog's global logger has not been configured for test correlation. The {nameof(SerilogTestCorrelator)} will not be able to collect LogEvents. This may be because you did not call {nameof(SerilogTestCorrelator)}.{nameof(SerilogTestCorrelator.ConfigureGlobalLoggerForTestCorrelation)}(), or because other code has overwritten {nameof(Serilog)}.{nameof(Log)}.{nameof(Log.Logger)} since you did.")
        {
        }
    }
}