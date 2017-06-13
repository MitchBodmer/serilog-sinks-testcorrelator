using System;
using FluentAssertions;
using Serilog;
using Serilog.Context;
using Serilog.Events;
using Xunit;

namespace SerilogTestCorrelation.Tests
{
    public partial class SerilogTestCorrelatorTests
    {
        [Fact]
        public void SerilogTestCorrelator_allows_you_to_filter_to_LogEvents_emitted_within_a_context()
        {
            Log.Information("");
            Log.Warning("");
            Log.Error("");

            using (SerilogTestCorrelator.CreateTestCorrelationContext())
            {
                Log.Information("");
                Log.Warning("");
                Log.Error("");
            }

            Guid testCorrelationContextGuid;

            using (var context = SerilogTestCorrelator.CreateTestCorrelationContext())
            {
                Log.Information("");
                Log.Warning("");
                Log.Error("");

                testCorrelationContextGuid = context.Guid;
            }

            SerilogTestCorrelator.GetLogEventsFromTestCorrelationContext(testCorrelationContextGuid)
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
        public void SerilogTestCorrelator_receives_LogEvents_of_all_LogEventLevels(LogEventLevel logEventLevel)
        {
            using (var context = SerilogTestCorrelator.CreateTestCorrelationContext())
            {
                Log.Write(logEventLevel, "");

                SerilogTestCorrelator.GetLogEventsFromTestCorrelationContext(context.Guid).Should().ContainSingle();
            }
        }

        [Fact]
        public void SerilogTestCorrelator_enriches_LogEvents_from_LogContext()
        {
            using (var context = SerilogTestCorrelator.CreateTestCorrelationContext())
            {
                const string propertyName = "Property name";

                using (LogContext.PushProperty(propertyName, new object()))
                {
                    Log.Information("");
                }

                SerilogTestCorrelator.GetLogEventsFromTestCorrelationContext(context.Guid)
                    .Should()
                    .ContainSingle()
                    .Which.Properties.Keys.Should()
                    .ContainSingle(key => key == propertyName);
            }
        }
    }
}
