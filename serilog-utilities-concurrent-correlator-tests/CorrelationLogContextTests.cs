using System;
using FluentAssertions;
using Xunit;

namespace Serilog.Utilities.ConcurrentCorrelator.Tests
{
    public class CorrelationLogContextTests
    {
        [Fact]
        public void A_CorrelationLogContext_enriches_logEvents_logged_inside_its_scope()
        {
            using (var correlationLogContext = new CorrelationLogContext())
            {
                Log.Logger.Information("This log message template.");

                SerilogLogEvents.Bag.WithCorrelationLogContextGuid(correlationLogContext.Guid)
                    .Should()
                    .OnlyContain(logEvent => logEvent.MessageTemplate.Text == "This log message template.");
            }
        }

        [Fact]
        public void A_CorrelationLogContext_does_not_enrich_logEvents_logged_outside_its_scope()
        {
            Guid correlationLogContextGuid;

            using (var correlationLogContext = new CorrelationLogContext())
            {
                correlationLogContextGuid = correlationLogContext.Guid;
            }

            Log.Logger.Information("This log message template.");

            SerilogLogEvents.Bag.WithCorrelationLogContextGuid(correlationLogContextGuid)
                .Should()
                .NotContain(logEvent => logEvent.MessageTemplate.Text == "This log message template.");
        }
    }
}
