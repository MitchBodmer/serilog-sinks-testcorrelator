using System.Linq;
using FluentAssertions;
using Serilog;
using Xunit;

namespace SerilogTestCorrelation.Tests
{
    public partial class SerilogTestCorrelatorTests
    {
        [Fact]
        public void GetLogEventsFromTestCorrelationContext_returns_empty_if_no_LogEvents_have_been_emitted()
        {
            using (var context = TestCorrelator.CreateTestCorrelationContext())
            {
                TestCorrelator.GetLogEventsFromTestCorrelationContext(context.Guid).Should().BeEmpty();
            }
        }

        [Fact]
        public void
            GetLogEventsFromTestCorrelationContext_returns_empty_if_no_LogEvents_have_been_emitted_within_the_context()
        {
            Log.Information("");

            using (var context = TestCorrelator.CreateTestCorrelationContext())
            {
                TestCorrelator.GetLogEventsFromTestCorrelationContext(context.Guid).Should().BeEmpty();
            }
        }

        [Fact]
        public void
            GetLogEventsFromTestCorrelationContext_returns_all_LogEvents_that_have_been_emitted_within_the_context()
        {
            using (var context = TestCorrelator.CreateTestCorrelationContext())
            {
                const int expectedCount = 4;

                foreach (var unused in Enumerable.Range(0, expectedCount))
                {
                    Log.Information("");
                }

                TestCorrelator.GetLogEventsFromTestCorrelationContext(context.Guid)
                    .Should().HaveCount(expectedCount);
            }
        }
    }
}