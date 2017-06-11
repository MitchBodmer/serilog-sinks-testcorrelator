using System.Linq;
using FluentAssertions;
using Xunit;

namespace Serilog.Utilities.ConcurrentCorrelator.Tests
{
    public partial class TestSerilogLogEventsTests
    {
        [Fact]
        public void GetLogEventsFromTestCorrelationContext_returns_empty_if_no_LogEvents_have_been_emitted()
        {
            using (var context = TestSerilogLogEvents.CreateTestCorrelationContext())
            {
                TestSerilogLogEvents.GetLogEventsFromTestCorrelationContext(context.Guid).Should().BeEmpty();
            }
        }

        [Fact]
        public void
            GetLogEventsFromTestCorrelationContext_returns_empty_if_no_LogEvents_have_been_emitted_with_the_context_identifier()
        {
            Log.Information("");

            using (var context = TestSerilogLogEvents.CreateTestCorrelationContext())
            {
                TestSerilogLogEvents.GetLogEventsFromTestCorrelationContext(context.Guid).Should().BeEmpty();
            }
        }

        [Fact]
        public void
            GetLogEventsFromTestCorrelationContext_returns_all_LogEvents_that_have_been_emitted_with_the_context_identifier()
        {
            using (var context = TestSerilogLogEvents.CreateTestCorrelationContext())
            {
                const int expectedCount = 4;

                foreach (var unused in Enumerable.Range(0, expectedCount))
                {
                    Log.Information("");
                }

                TestSerilogLogEvents.GetLogEventsFromTestCorrelationContext(context.Guid).Should().HaveCount(expectedCount);
            }
        }
    }
}