using System;
using System.Collections.Generic;
using System.Linq;
using Serilog.Events;

namespace Serilog.Utilities.ConcurrentCorrelator
{
    public static class EnumerableOfLogEventExtensions
    {
        public static IEnumerable<LogEvent> WithCorrelationLogContextGuid(
            this IEnumerable<LogEvent> logEvents, Guid correlationLogContextGuid)
        {
            return logEvents.Where(
                logEvent =>
                {
                    if (logEvent.Properties.ContainsKey("CorrelationGuid"))
                    {
                        Guid result;

                        Guid.TryParse(logEvent.Properties["CorrelationGuid"].ToString(), out result);

                        return result == correlationLogContextGuid;
                    }

                    return false;
                });
        }
    }
}