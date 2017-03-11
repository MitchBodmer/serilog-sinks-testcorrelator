using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Serilog.Events;
using Serilog.Parsing;
using Xunit;

namespace Serilog.Utilities.ConcurrentCorrelator.Tests
{
    public class EnumerableOfLogEventExtensionsTests
    {
        private LogEvent GetLogEventWithoutCorrelationGuid()
        {
            return new LogEvent(DateTimeOffset.Now,
                LogEventLevel.Information, null,
                new MessageTemplate("Message template.", new List<MessageTemplateToken>()),
                new List<LogEventProperty>());
        }

        private LogEvent GetLogEventWithCorrelationGuid(Guid correlationGuid)
        {
            return new LogEvent(DateTimeOffset.Now,
                LogEventLevel.Information, null,
                new MessageTemplate("Message template.", new List<MessageTemplateToken>()),
                new List<LogEventProperty>
                {
                    new LogEventProperty("CorrelationGuid", new ScalarValue(correlationGuid))
                });
        }

        [Fact]
        public void WithCorrelationLogContextGuid_returns_empty_if_no_log_events_have_been_logged()
        {
            new List<LogEvent>().WithCorrelationLogContextGuid(Guid.NewGuid()).Should().BeEmpty();
        }

        [Fact]
        public void
            WithCorrelationLogContextGuid_returns_empty_if_no_log_events_have_been_logged_with_the_correlation_guid()
        {
            new List<LogEvent>
            {
                GetLogEventWithCorrelationGuid(Guid.NewGuid())
            }.WithCorrelationLogContextGuid(Guid.NewGuid()).Should().BeEmpty();
        }

        [Fact]
        public void
            WithCorrelationLogContextGuid_returns_one_log_event_if_one_has_been_logged_with_the_correlation_guid()
        {
            var correlationGuid = Guid.NewGuid();

            var logEventWithCorrelationGuid = GetLogEventWithCorrelationGuid(correlationGuid);

            new List<LogEvent>
                {
                    logEventWithCorrelationGuid
                }.WithCorrelationLogContextGuid(correlationGuid)
                .Should()
                .OnlyContain(logEvent => logEvent == logEventWithCorrelationGuid);
        }

        [Fact]
        public void
            WithCorrelationLogContextGuid_returns_all_log_events_that_have_been_logged_with_the_correlation_guid()
        {
            var correlationGuid = Guid.NewGuid();

            var logEventsWithCorrelationGuid = new List<LogEvent>
            {
                GetLogEventWithCorrelationGuid(correlationGuid),
                GetLogEventWithCorrelationGuid(correlationGuid),
                GetLogEventWithCorrelationGuid(correlationGuid),
                GetLogEventWithCorrelationGuid(correlationGuid),
            };

            logEventsWithCorrelationGuid.WithCorrelationLogContextGuid(correlationGuid)
                .Should()
                .Contain(logEventsWithCorrelationGuid);
        }

        [Fact]
        public void WithCorrelationLogContextGuid_does_not_return_a_log_event_without_a_correlation_guid()
        {
            new List<LogEvent>
            {
                GetLogEventWithoutCorrelationGuid()
            }.WithCorrelationLogContextGuid(Guid.NewGuid()).Should().BeEmpty();
        }

        [Fact]
        public void
            WithCorrelationLogContextGuid_filters_all_log_events_that_do_not_have_the_correct_correlation_guid()
        {
            var correlationGuid = Guid.NewGuid();

            var logEventsWithCorrectCorrelationGuid = new List<LogEvent>
            {
                GetLogEventWithCorrelationGuid(correlationGuid),
                GetLogEventWithCorrelationGuid(correlationGuid),
                GetLogEventWithCorrelationGuid(correlationGuid),
                GetLogEventWithCorrelationGuid(correlationGuid),
            };

            var logEventsWithNoCorrelationGuid = new List<LogEvent>
            {
                GetLogEventWithoutCorrelationGuid(),
                GetLogEventWithoutCorrelationGuid(),
                GetLogEventWithoutCorrelationGuid(),
                GetLogEventWithoutCorrelationGuid(),
            };

            var logEventsWithWrongCorrelationGuid = new List<LogEvent>
            {
                GetLogEventWithCorrelationGuid(Guid.NewGuid()),
                GetLogEventWithCorrelationGuid(Guid.NewGuid()),
                GetLogEventWithCorrelationGuid(Guid.NewGuid()),
                GetLogEventWithCorrelationGuid(Guid.NewGuid()),
            };

            var allLogEvents =
                logEventsWithCorrectCorrelationGuid.Concat(
                    logEventsWithNoCorrelationGuid.Concat(logEventsWithWrongCorrelationGuid));

            allLogEvents.WithCorrelationLogContextGuid(correlationGuid)
                .Should()
                .Contain(logEventsWithCorrectCorrelationGuid);
        }
    }
}
