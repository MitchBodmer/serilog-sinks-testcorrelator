using System.Collections.Concurrent;
using Serilog.Events;

namespace Serilog.Utilities.ConcurrentCorrelator
{
    public static class SerilogLogEvents
    {
        public static readonly ConcurrentBag<LogEvent> Bag = new ConcurrentBag<LogEvent>();
    }
}