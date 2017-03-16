using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Serilog.Events;

namespace Serilog.Utilities.ConcurrentCorrelator
{
    public static class TestSerilogLogEvents
    {
        static readonly ConcurrentBag<LogEvent> bag = new ConcurrentBag<LogEvent>();

        static bool initialized;

        static readonly object initializedLock = new object();

        public static void Initialize()
        {
            lock (initializedLock)
            {
                if (initialized == false)
                {
                    Log.Logger =
                        new LoggerConfiguration().MinimumLevel.Verbose()
                            .WriteTo.ConcurrentBag(bag)
                            .Enrich.FromLogContext()
                            .CreateLogger();

                    initialized = true;
                }
            }
        }

        public static IEnumerable<LogEvent> WithCorrelationLogContextGuid(Guid correlationLogContextGuid)
        {
            return bag.Where(logEvent => logEvent.Properties.ContainsKey(correlationLogContextGuid.ToString()));
        }
    }
}