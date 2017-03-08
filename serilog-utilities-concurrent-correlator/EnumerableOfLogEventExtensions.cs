using System;
using System.Collections.Generic;
using System.Linq;
using Serilog.Events;

namespace Serilog.Utilities.ConcurrentCorrelator
{
    public static class EnumerableOfLogEventExtensions
    {
        public static IEnumerable<LogEvent> WithCorrelationLogContext(
            this IEnumerable<LogEvent> logEvents,
            CorrelationLogContext correlationLogContext)
        {
            return logEvents.Where(
                logEvent =>
                {
                    if (logEvent.Properties.ContainsKey("CorrelationId"))
                    {
                        Guid result;

                        Guid.TryParse(logEvent.Properties["CorrelationId"].ToString(), out result);
                        return result == correlationLogContext.Id;
                    }

                    return false;
                });
        }
    }
}