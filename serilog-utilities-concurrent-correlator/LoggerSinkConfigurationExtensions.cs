using System.Collections.Concurrent;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;

namespace Serilog.Utilities.ConcurrentCorrelator
{
    static class LoggerSinkConfigurationExtensions
    {
        internal static LoggerConfiguration ConcurrentBag(this LoggerSinkConfiguration sinkConfiguration,
            ConcurrentBag<LogEvent> concurrentBag, LogEventLevel restrictedToMinimumLevel = LogEventLevel.Verbose,
            LoggingLevelSwitch levelSwitch = null)
        {
            return sinkConfiguration.Sink(new ConcurrentBagSink(concurrentBag), restrictedToMinimumLevel, levelSwitch);
        }
    }
}