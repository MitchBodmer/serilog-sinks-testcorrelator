using System;
using System.Collections.Concurrent;
using System.Reactive.Linq;
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
            return sinkConfiguration.Observers(events => events.Do(concurrentBag.Add).Subscribe(),
                restrictedToMinimumLevel, levelSwitch);
        }
    }
}