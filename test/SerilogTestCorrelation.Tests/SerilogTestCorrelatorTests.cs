using System;
using FluentAssertions;
using Serilog;
using Serilog.Context;
using Serilog.Events;
using Xunit;

namespace SerilogTestCorrelation.Tests
{
    public partial class TestCorrelatorTests
    {
        [Fact]
        public void TestCorrelator_allows_you_to_filter_to_LogEvents_emitted_within_a_context()
        {
            Log.Information("");
            Log.Warning("");
            Log.Error("");

            using (TestCorrelator.CreateContext())
            {
                Log.Information("");
                Log.Warning("");
                Log.Error("");
            }

            Guid testCorrelationContextGuid;

            using (var context = TestCorrelator.CreateContext())
            {
                Log.Information("");
                Log.Warning("");
                Log.Error("");

                testCorrelationContextGuid = context.Guid;
            }

            TestCorrelator.GetLogEventsFromContext(testCorrelationContextGuid)
                .Should().ContainSingle(logEvent => logEvent.Level == LogEventLevel.Information)
                .And.ContainSingle(logEvent => logEvent.Level == LogEventLevel.Warning)
                .And.ContainSingle(logEvent => logEvent.Level == LogEventLevel.Error)
                .And.HaveCount(3);
        }

        [Theory]
        [InlineData(LogEventLevel.Information)]
        [InlineData(LogEventLevel.Debug)]
        [InlineData(LogEventLevel.Error)]
        [InlineData(LogEventLevel.Fatal)]
        [InlineData(LogEventLevel.Verbose)]
        [InlineData(LogEventLevel.Warning)]
        public void TestCorrelator_receives_LogEvents_of_all_LogEventLevels(LogEventLevel logEventLevel)
        {
            using (var context = TestCorrelator.CreateContext())
            {
                Log.Write(logEventLevel, "");

                TestCorrelator.GetLogEventsFromContext(context.Guid).Should().ContainSingle();
            }
        }

        [Fact]
        public void TestCorrelator_enriches_LogEvents_with_LogContext()
        {
            using (var context = TestCorrelator.CreateContext())
            {
                const string propertyName = "Property name";

                using (LogContext.PushProperty(propertyName, new object()))
                {
                    Log.Information("");
                }

                TestCorrelator.GetLogEventsFromContext(context.Guid)
                    .Should().ContainSingle().Which.Properties.Keys
                    .Should().ContainSingle().Which.Should().Be(propertyName);
            }
        }
    }
}