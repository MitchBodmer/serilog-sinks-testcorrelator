using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Serilog.Core;
using Serilog.Events;

namespace Serilog.Utilities.ConcurrentCorrelator
{
    class TestCorrelationContextSink : ILogEventSink
    {
        readonly ConcurrentDictionary<Guid, ConcurrentBag<LogEvent>> testCorrelationContextGuidBags = new ConcurrentDictionary<Guid, ConcurrentBag<LogEvent>>();

        public void Emit(LogEvent logEvent)
        {
            foreach (var guid in testCorrelationContextGuidBags.Keys)
            {
                if (LogicalCallContext.Contains(guid))
                {
                    testCorrelationContextGuidBags[guid].Add(logEvent);
                }
            }
        }

        public TestCorrelationContext CreateTestCorrelationContext()
        {
            var testCorrelationContext = new TestCorrelationContext();

            testCorrelationContextGuidBags.GetOrAdd(testCorrelationContext.Guid, new ConcurrentBag<LogEvent>());

            return testCorrelationContext;
        }

        public IEnumerable<LogEvent> GetLogEventsFromTestCorrelationContext(Guid testCorrelationContextGuid)
        {
            return testCorrelationContextGuidBags[testCorrelationContextGuid];
        }
    }
}