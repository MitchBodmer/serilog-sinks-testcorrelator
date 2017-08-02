using System;
using Serilog;

namespace SerilogTestCorrelation
{
    /// <summary>
    /// Exception thrown when Serilog's global logger is not configured for test correlation and a method on <seealso cref="TestCorrelator"/> is called that requires it.
    /// </summary>
    public class GlobalLoggerNotConfiguredForTestCorrelationException : Exception
    {
        internal GlobalLoggerNotConfiguredForTestCorrelationException() :
            base(
                $"Serilog's global logger has not been configured for test correlation. The {nameof(TestCorrelator)} will not be able to collect LogEvents. This may be because you did not call {nameof(TestCorrelator)}.{nameof(TestCorrelator.ConfigureGlobalLoggerForTestCorrelation)}(), or because other code has overwritten {nameof(Serilog)}.{nameof(Log)}.{nameof(Log.Logger)} since you did.")
        {
        }
    }
}