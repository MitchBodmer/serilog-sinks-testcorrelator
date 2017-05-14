using System;
using FluentAssertions;
using Serilog.Events;
using Xunit;

namespace Serilog.Utilities.ConcurrentCorrelator.Tests
{
    public partial class TestSerilogLogEventsTests
    {
        [Fact]
        public void TestSerilogEvents_allows_you_to_filter_all_LogEvents_without_the_correct_context_identifier()
        {
            Log.Information("");
            Log.Warning("");
            Log.Error("");

            using (TestSerilogLogEvents.EstablishTestLogContext())
            {
                Log.Information("");
                Log.Warning("");
                Log.Error("");
            }

            Guid testLogContextIdentifier;

            using (var context = TestSerilogLogEvents.EstablishTestLogContext())
            {
                Log.Information("");
                Log.Warning("");
                Log.Error("");

                testLogContextIdentifier = context.Guid;
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
        public void TestSerilogLogEvents_receives_LogEvents_of_all_LogEventLevels(LogEventLevel logEventLevel)
        {
            using (var context = TestSerilogLogEvents.EstablishTestLogContext())
            {
                Log.Write(logEventLevel, "");

                TestSerilogLogEvents.GetLogEventsWithContextIdentifier(context.Guid).Should().ContainSingle();
            }
        }
    }
}
