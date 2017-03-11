using System;
using System.Collections.Concurrent;
using System.Reactive.Linq;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;

namespace Serilog.Utilities.ConcurrentCorrelator
{
    public static class LoggerSinkConfigurationExtensions
    {
        internal static LoggerConfiguration ConcurrentBag(this LoggerSinkConfiguration sinkConfiguration,
            ConcurrentBag<LogEvent> concurrentBag, LogEventLevel restrictedToMinimumLevel = LogEventLevel.Verbose,
            LoggingLevelSwitch levelSwitch = null)
        {
            if (concurrentBag == null)
            {
                throw new ArgumentNullException(nameof(concurrentBag));
            }
            if (sinkConfiguration == null)
            {
                throw new ArgumentNullException(nameof(sinkConfiguration));
            }

            return sinkConfiguration.Observers(events => events.Do(concurrentBag.Add).Subscribe(),
                restrictedToMinimumLevel, levelSwitch);
        }
    }
}