using System;
using System.Collections.Generic;
using FluentAssertions;
using Serilog.Events;
using Serilog.Parsing;
using Xunit;

namespace Serilog.Utilities.ConcurrentCorrelator.Tests
{
    public class EnumerableOfLogEventExtensionsTests
    {
        [Fact]
        public void WithCorrelationLogContextGuid_returns_empty_if_no_log_events_have_been_logged()
        {
            new List<LogEvent>().WithCorrelationLogContextGuid(Guid.NewGuid()).Should().BeEmpty();
        }

        [Fact]
        public void WithCorrelationLogContextGuid_does_not_return_a_log_event_without_a_correlation_guid()
        {
            var logEventWithoutCorrelationGuid = new LogEvent(DateTimeOffset.Now, LogEventLevel.Information, null,
                new MessageTemplate("Message template.", new List<MessageTemplateToken>()),
                new List<LogEventProperty>());

            new List<LogEvent>
            {
                logEventWithoutCorrelationGuid
            }.WithCorrelationLogContextGuid(Guid.NewGuid()).Should().BeEmpty();
        }
    }
}
