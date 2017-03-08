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
        public static LoggerConfiguration ProducerConsumerCollection(this LoggerSinkConfiguration sinkConfiguration, IProducerConsumerCollection<LogEvent> logEventsList, LogEventLevel restrictedToMinimumLevel = LogEventLevel.Verbose, LoggingLevelSwitch levelSwitch = null)
        {
            if (logEventsList == null)
            {
                throw new ArgumentNullException(nameof(logEventsList));
            }
            if (sinkConfiguration == null)
            {
                throw new ArgumentNullException(nameof(sinkConfiguration));
            }

            return sinkConfiguration.Observers(events => events.Do(logEvent => logEventsList.TryAdd(logEvent)).Subscribe(), restrictedToMinimumLevel, levelSwitch);
        }
    }
}