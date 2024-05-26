using System;
using System.Collections.Generic;
using Serilog.Events;

namespace Serilog.Sinks.TestCorrelator;

class ContextGuidDecoratedLogEvent
{
    public ContextGuidDecoratedLogEvent(LogEvent logEvent, IEnumerable<Guid> contextGuids)
    {
        LogEvent = logEvent;
        ContextGuids = contextGuids;
    }

    public LogEvent LogEvent { get; }

    public IEnumerable<Guid> ContextGuids { get; }
}