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
    }
}
