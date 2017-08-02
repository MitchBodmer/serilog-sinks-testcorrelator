using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Serilog.Core;
using Serilog.Events;

namespace SerilogTestCorrelation
{
    class TestCorrelatorContextSink : ILogEventSink
    {
        readonly ConcurrentDictionary<Guid, ConcurrentBag<LogEvent>> testCorrelationContextGuidBags =
            new ConcurrentDictionary<Guid, ConcurrentBag<LogEvent>>();

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

        public TestCorrelatorContext CreateTestCorrelationContext()
        {
            var testCorrelationContext = new TestCorrelatorContext();

            testCorrelationContextGuidBags.GetOrAdd(testCorrelationContext.Guid, new ConcurrentBag<LogEvent>());

            return testCorrelationContext;
        }

        public IEnumerable<LogEvent> GetLogEventsFromTestCorrelationContext(Guid testCorrelationContextGuid)
        {
            return testCorrelationContextGuidBags[testCorrelationContextGuid];
        }
    }
}