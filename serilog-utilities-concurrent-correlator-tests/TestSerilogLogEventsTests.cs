using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Serilog.Events;
using Serilog.Fakes;
using Serilog.Parsing;
using Xunit;

namespace Serilog.Utilities.ConcurrentCorrelator.Tests
{
    [TestClass]
    public class TestSerilogLogEventsTests
    {
        public TestSerilogLogEventsTests()
        {
            TestSerilogLogEvents.ConfigureGlobalLoggerForTesting();
        }

        static LogEvent GetLogEventWithoutCorrelationGuid()
        {
            return new LogEvent(DateTimeOffset.Now,
                LogEventLevel.Information, null,
                new MessageTemplate("Message template.", Enumerable.Empty<MessageTemplateToken>()),
                new List<LogEventProperty>());
        }

        static LogEvent GetLogEventWithCorrelationGuid(Guid correlationGuid)
        {
            return new LogEvent(DateTimeOffset.Now,
                LogEventLevel.Information, null,
                new MessageTemplate("Message template.", Enumerable.Empty<MessageTemplateToken>()),
                new List<LogEventProperty>
                {
                    new LogEventProperty(correlationGuid.ToString(), new ScalarValue(null))
                });
        }

        [Fact]
        [Test]
        [TestMethod]
        public void WithCorrelationLogContextGuid_returns_empty_if_no_logEvents_have_been_logged()
        {
            TestSerilogLogEvents.WithCorrelationLogContextGuid(Guid.NewGuid()).Should().BeEmpty();
        }

        [Fact]
        [Test]
        [TestMethod]
        public void
            WithCorrelationLogContextGuid_returns_empty_if_no_LogEvents_have_been_logged_with_the_correlation_guid()
        {
            Log.Logger.Write(GetLogEventWithCorrelationGuid(Guid.NewGuid()));

            TestSerilogLogEvents.WithCorrelationLogContextGuid(Guid.NewGuid()).Should().BeEmpty();
        }

        [Fact]
        [Test]
        [TestMethod]
        public void
            WithCorrelationLogContextGuid_returns_one_LogEvent_if_one_has_been_logged_with_the_correlation_guid()
        {
            var correlationGuid = Guid.NewGuid();

            var logEventWithCorrelationGuid = GetLogEventWithCorrelationGuid(correlationGuid);

            Log.Logger.Write(logEventWithCorrelationGuid);

            TestSerilogLogEvents.WithCorrelationLogContextGuid(correlationGuid)
                .Should()
                .OnlyContain(logEvent => logEvent == logEventWithCorrelationGuid);
        }

        [Fact]
        [Test]
        [TestMethod]
        public void
            WithCorrelationLogContextGuid_returns_all_LogEvents_that_have_been_logged_with_the_correlation_guid()
        {
            var correlationGuid = Guid.NewGuid();

            var logEventsWithCorrelationGuid = new List<LogEvent>
            {
                GetLogEventWithCorrelationGuid(correlationGuid),
                GetLogEventWithCorrelationGuid(correlationGuid),
                GetLogEventWithCorrelationGuid(correlationGuid),
                GetLogEventWithCorrelationGuid(correlationGuid),
            };

            foreach (var logEvent in logEventsWithCorrelationGuid)
            {
                Log.Logger.Write(logEvent);
            }

            TestSerilogLogEvents.WithCorrelationLogContextGuid(correlationGuid)
                .Should()
                .Contain(logEventsWithCorrelationGuid);
        }

        [Fact]
        [Test]
        [TestMethod]
        public void WithCorrelationLogContextGuid_does_not_return_a_LogEvent_without_a_correlation_guid()
        {
            Log.Logger.Write(GetLogEventWithoutCorrelationGuid());

            TestSerilogLogEvents.WithCorrelationLogContextGuid(Guid.NewGuid()).Should().BeEmpty();
        }

        [Fact]
        [Test]
        [TestMethod]
        public void
            WithCorrelationLogContextGuid_filters_all_LogEvents_without_the_correct_correlation_guid()
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

            foreach (var logEvent in allLogEvents)
            {
                Log.Logger.Write(logEvent);
            }

            TestSerilogLogEvents.WithCorrelationLogContextGuid(correlationGuid)
                .Should()
                .Contain(logEventsWithCorrectCorrelationGuid);
        }

        [Fact]
        [Test]
        [TestMethod]
        public void WithCorrelationLogContextGuid_throws_an_exception_if_ConfigureGlobalLoggerForTesting_has_not_been_called()
        {
            using (ShimsContext.Create())
            {
                ShimLog.LoggerGet = () => null;

                Action throwingAction = () => TestSerilogLogEvents.WithCorrelationLogContextGuid(Guid.NewGuid());

                throwingAction.ShouldThrow<Exception>();
            }
        }

        [Fact]
        [Test]
        [TestMethod]
        public void After_ConfigureGlobalLoggerForTesting_is_called_the_global_logger_is_not_a_SilentLogger()
        {
            Log.Logger.GetType().FullName.Should().NotBe("Serilog.Core.Pipeline.SilentLogger");
        }

        [Fact]
        [Test]
        [TestMethod]
        public void After_ConfigureGlobalLoggerForTesting_is_called_the_global_logger_is_a_Logger()
        {
            Log.Logger.GetType().FullName.Should().Be("Serilog.Core.Logger");
        }

        [Xunit.Theory]
        [InlineData(LogEventLevel.Information)]
        [InlineData(LogEventLevel.Debug)]
        [InlineData(LogEventLevel.Error)]
        [InlineData(LogEventLevel.Fatal)]
        [InlineData(LogEventLevel.Verbose)]
        [InlineData(LogEventLevel.Warning)]
        [TestCase(LogEventLevel.Information)]
        [TestCase(LogEventLevel.Debug)]
        [TestCase(LogEventLevel.Error)]
        [TestCase(LogEventLevel.Fatal)]
        [TestCase(LogEventLevel.Verbose)]
        [TestCase(LogEventLevel.Warning)]
        [DataTestMethod]
        [DataRow(LogEventLevel.Information)]
        [DataRow(LogEventLevel.Debug)]
        [DataRow(LogEventLevel.Error)]
        [DataRow(LogEventLevel.Fatal)]
        [DataRow(LogEventLevel.Verbose)]
        [DataRow(LogEventLevel.Warning)]
        public void After_ConfigureGlobalLoggerForTesting_is_called_the_static_SerilogLogEvents_bag_receives_LogEvents_of_all_LogEventLevels(
            LogEventLevel level)
        {
            using (var correlationLogContext = new CorrelationLogContext())
            {
                var uniqueMessageTemplate = Guid.NewGuid().ToString();

                Log.Logger.Write(new LogEvent(DateTimeOffset.Now, level, null,
                    new MessageTemplate(uniqueMessageTemplate, Enumerable.Empty<MessageTemplateToken>()),
                    Enumerable.Empty<LogEventProperty>()));

                TestSerilogLogEvents.WithCorrelationLogContextGuid(correlationLogContext.Guid).Should()
                    .Contain(logEvent => logEvent.MessageTemplate.Text == uniqueMessageTemplate);
            }
        }

        [Fact]
        [Test]
        [TestMethod]
        public void Calling_ConfigureGlobalLoggerForTesting_twice_does_not_clear_all_log_events()
        {
            var correlationGuid = Guid.NewGuid();

            var logEventWithCorrelationGuid = GetLogEventWithCorrelationGuid(correlationGuid);

            Log.Logger.Write(logEventWithCorrelationGuid);

            TestSerilogLogEvents.ConfigureGlobalLoggerForTesting();

            TestSerilogLogEvents.WithCorrelationLogContextGuid(correlationGuid)
                .Should()
                .OnlyContain(logEvent => logEvent == logEventWithCorrelationGuid);
        }

        [Fact]
        [Test]
        [TestMethod]
        public void Calling_ConfigureGlobalLoggerForTesting_is_idempotent()
        {
            var oldLogger = Log.Logger;

            TestSerilogLogEvents.ConfigureGlobalLoggerForTesting();

            Log.Logger.Should().Be(oldLogger);
        }
    }
}
