using FluentAssertions;
using Xunit;

namespace Serilog.Utilities.ConcurrentCorrelator.Tests
{
    public class GlobalLoggerNotConfiguredForTestCorrelationExceptionTests
    {
        [Fact]
        public void A_GlobalLoggerNotConfiguredForTestCorrelationException_should_have_the_correct_message()
        {
            new GlobalLoggerNotConfiguredForTestCorrelationException().Message.Should().Be("Serilog's global logger has not been configured for test correlation. This can either be because you did not call ConfigureGlobalLoggerForTestCorrelation, or because other code has overwritten Logger since you did.");
        }
    }
}
