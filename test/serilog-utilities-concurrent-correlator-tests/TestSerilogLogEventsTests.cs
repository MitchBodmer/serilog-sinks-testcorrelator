using System;
using FluentAssertions;
using Serilog.Context;
using Serilog.Events;
using Xunit;

namespace Serilog.Utilities.ConcurrentCorrelator.Tests
{
    public partial class TestSerilogLogEventsTests
    {
        [Fact]
        public void TestSerilogLogEvents_allows_you_to_filter_to_LogEvents_emitted_within_a_context()
        {
            Log.Information("");
            Log.Warning("");
            Log.Error("");

            using (TestSerilogLogEvents.CreateTestCorrelationContext())
            {
                Log.Information("");
                Log.Warning("");
                Log.Error("");
            }

            Guid testCorrelationContextGuid;

            using (var context = TestSerilogLogEvents.CreateTestCorrelationContext())
            {
                Log.Information("");
                Log.Warning("");
                Log.Error("");

                testCorrelationContextGuid = context.Guid;
            }

            TestSerilogLogEvents.GetLogEventsFromTestCorrelationContext(testCorrelationContextGuid)
                .Should()
                .ContainSingle(logEvent => logEvent.Level == LogEventLevel.Information).And
                .ContainSingle(logEvent => logEvent.Level == LogEventLevel.Warning).And
                .ContainSingle(logEvent => logEvent.Level == LogEventLevel.Error).And
                .HaveCount(3);
        }

        [Theory]
        [InlineData(LogEventLevel.Information)]
        [InlineData(LogEventLevel.Debug)]
        [InlineData(LogEventLevel.Error)]
        [InlineData(LogEventLevel.Fatal)]
        [InlineData(LogEventLevel.Verbose)]
        [InlineData(LogEventLevel.Warning)]
        public void TestSerilogLogEvents_receives_LogEvents_of_all_LogEventLevels(LogEventLevel logEventLevel)
        {
            using (var context = TestSerilogLogEvents.CreateTestCorrelationContext())
            {
                Log.Write(logEventLevel, "");

                TestSerilogLogEvents.GetLogEventsFromTestCorrelationContext(context.Guid).Should().ContainSingle();
            }
        }

        [Fact]
        public void TestSerilogLogEvents_enriches_LogEvents_from_LogContext()
        {
            using (var context = TestSerilogLogEvents.CreateTestCorrelationContext())
            {
                const string propertyName = "Property name";

                using (LogContext.PushProperty(propertyName, new object()))
                {
                    Log.Information("");
                }

                TestSerilogLogEvents.GetLogEventsFromTestCorrelationContext(context.Guid)
                    .Should()
                    .ContainSingle()
                    .Which.Properties.Keys.Should()
                    .ContainSingle(key => key == propertyName);
            }
        }
    }
}
