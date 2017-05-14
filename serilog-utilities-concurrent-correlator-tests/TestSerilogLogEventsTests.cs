using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Serilog.Events;
using Serilog.Parsing;
using Xunit;

namespace Serilog.Utilities.ConcurrentCorrelator.Tests
{
    [TestClass]
    public partial class TestSerilogLogEventsTests
    {
        [Fact]
        [Test]
        [TestMethod]
        public void TestSerilogEvents_allows_you_to_filter_all_LogEvents_without_the_correct_context_identifier()
        {
            ITestLogContextIdentifier testLogContextIdentifier;

            Log.Information("");
            Log.Warning("");
            Log.Error("");

            using (TestSerilogLogEvents.EstablishTestLogContext())
            {
                Log.Information("");
                Log.Warning("");
                Log.Error("");
            }

            using (var context = TestSerilogLogEvents.EstablishTestLogContext())
            {
                Log.Information("");
                Log.Warning("");
                Log.Error("");

                testLogContextIdentifier = context.Identifier;
            }

            TestSerilogLogEvents.GetLogEventsWithContextIdentifier(testLogContextIdentifier)
                .Should()
                .Contain(logEvent => logEvent.Level == LogEventLevel.Information)
                .And
                .Contain(logEvent => logEvent.Level == LogEventLevel.Warning)
                .And
                .Contain(logEvent => logEvent.Level == LogEventLevel.Error)
                .And
                .HaveCount(3);
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
        public void TestSerilogLogEvents_receives_LogEvents_of_all_LogEventLevels(LogEventLevel logEventLevel)
        {
            using (var context = TestSerilogLogEvents.EstablishTestLogContext())
            {
                WriteLogEventWithLogEventLevel(logEventLevel);

                TestSerilogLogEvents.GetLogEventsWithContextIdentifier(context.Identifier).Should().ContainSingle();
            }
        }

        static void WriteLogEventWithLogEventLevel(LogEventLevel logEventLevel)
        {
            Log.Write(new LogEvent(
                DateTimeOffset.Now,
                logEventLevel,
                null,
                new MessageTemplate("", Enumerable.Empty<MessageTemplateToken>()),
                new List<LogEventProperty>()));
        }
    }
}
