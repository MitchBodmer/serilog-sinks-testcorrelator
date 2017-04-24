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
            return new LogEvent(
                DateTimeOffset.Now,
                LogEventLevel.Information, 
                null,
                new MessageTemplate("", Enumerable.Empty<MessageTemplateToken>()),
                new List<LogEventProperty>());
        }

        static LogEvent GetLogEventWithCorrelationGuid(Guid correlationGuid)
        {
            return new LogEvent(
                DateTimeOffset.Now,
                LogEventLevel.Information, 
                null,
                new MessageTemplate("", Enumerable.Empty<MessageTemplateToken>()),
                new List<LogEventProperty>
                {
                    new LogEventProperty(correlationGuid.ToString(), new ScalarValue(null))
                });
        }

        static LogEvent GetLogEventWithLogEventLevel(LogEventLevel logEventLevel)
        {
            return new LogEvent(
                DateTimeOffset.Now,
                logEventLevel, 
                null,
                new MessageTemplate("", Enumerable.Empty<MessageTemplateToken>()),
                new List<LogEventProperty>());
        }

        [Fact]
        [Test]
        [TestMethod]
        public void WithTestLogContextGuid_returns_empty_if_no_LogEvents_have_been_logged()
        {
            TestSerilogLogEvents.WithTestLogContextGuid(Guid.NewGuid()).Should().BeEmpty();
        }

        [Fact]
        [Test]
        [TestMethod]
        public void
            WithTestLogContextGuid_returns_empty_if_no_LogEvents_have_been_logged_with_the_correlation_guid()
        {
            Log.Write(GetLogEventWithCorrelationGuid(Guid.NewGuid()));

            TestSerilogLogEvents.WithTestLogContextGuid(Guid.NewGuid()).Should().BeEmpty();
        }

        [Fact]
        [Test]
        [TestMethod]
        public void
            WithTestLogContextGuid_returns_one_LogEvent_if_one_has_been_logged_with_the_correlation_guid()
        {
            var correlationGuid = Guid.NewGuid();

            var logEventWithCorrelationGuid = GetLogEventWithCorrelationGuid(correlationGuid);

            Log.Write(logEventWithCorrelationGuid);

            TestSerilogLogEvents.WithTestLogContextGuid(correlationGuid).Should().ContainSingle();
        }

        [Fact]
        [Test]
        [TestMethod]
        public void
            WithTestLogContextGuid_returns_all_LogEvents_that_have_been_logged_with_the_correlation_guid()
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
                Log.Write(logEvent);
            }

            TestSerilogLogEvents.WithTestLogContextGuid(correlationGuid).Should().BeEquivalentTo(logEventsWithCorrelationGuid);
        }

        [Fact]
        [Test]
        [TestMethod]
        public void WithTestLogContextGuid_does_not_return_a_LogEvent_without_a_correlation_guid()
        {
            Log.Write(GetLogEventWithoutCorrelationGuid());

            TestSerilogLogEvents.WithTestLogContextGuid(Guid.NewGuid()).Should().BeEmpty();
        }

        [Fact]
        [Test]
        [TestMethod]
        public void
            WithTestLogContextGuid_filters_all_LogEvents_without_the_correct_correlation_guid()
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
                Log.Write(logEvent);
            }

            TestSerilogLogEvents.WithTestLogContextGuid(correlationGuid).Should().BeEquivalentTo(logEventsWithCorrectCorrelationGuid);
        }

        [Fact]
        [Test]
        [TestMethod]
        public void EstablishContext_throws_a_TestSerilogEventsNotConfiguredException_if_the_global_logger_is_not_configured_for_testing()
        {
            using (ShimsContext.Create())
            {
                SetGlobalLoggerToNull();

                Action throwingAction = () => TestSerilogLogEvents.EstablishTestLogContext();

                throwingAction.ShouldThrow<TestSerilogLogEvents.TestSerilogEventsNotConfiguredException>()
                    .WithMessage(
                        "The global logger has not been configured for testing. This can either be because you did not call TestSerilogEvents.ConfigureGlobalLoggerForTesting(), or because other code has overridden the global logger.");
            }
        }

        [Fact]
        [Test]
        [TestMethod]
        public void WithTestLogContextGuid_throws_a_TestSerilogEventsNotConfiguredException_if_the_global_logger_is_not_configured_for_testing()
        {
            using (ShimsContext.Create())
            {
                SetGlobalLoggerToNull();

                Action throwingAction = () => TestSerilogLogEvents.WithTestLogContextGuid(Guid.NewGuid());

                throwingAction.ShouldThrow<TestSerilogLogEvents.TestSerilogEventsNotConfiguredException>()
                    .WithMessage(
                        "The global logger has not been configured for testing. This can either be because you did not call TestSerilogEvents.ConfigureGlobalLoggerForTesting(), or because other code has overridden the global logger.");
            }
        }

        static void SetGlobalLoggerToNull()
        {
            ShimLog.LoggerGet = () => null;
        }

        [Fact]
        [Test]
        [TestMethod]
        public void After_ConfigureGlobalLoggerForTesting_is_called_the_global_logger_is_a_Logger()
        {
            Log.Logger.Should().BeOfType<Core.Logger>();
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
            LogEventLevel logEventLevel)
        {
            using (var correlationLogContext = TestSerilogLogEvents.EstablishTestLogContext())
            {
                Log.Write(GetLogEventWithLogEventLevel(logEventLevel));

                TestSerilogLogEvents.WithTestLogContextGuid(correlationLogContext.Guid).Should().ContainSingle();
            }
        }

        [Fact]
        [Test]
        [TestMethod]
        public void Calling_ConfigureGlobalLoggerForTesting_twice_does_not_clear_all_log_events()
        {
            var correlationGuid = Guid.NewGuid();

            var logEventWithCorrelationGuid = GetLogEventWithCorrelationGuid(correlationGuid);

            Log.Write(logEventWithCorrelationGuid);

            TestSerilogLogEvents.ConfigureGlobalLoggerForTesting();

            TestSerilogLogEvents.WithTestLogContextGuid(correlationGuid).Should().ContainSingle();
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
