using FluentAssertions;
using Xunit;

namespace SerilogTestCorrelation.Tests
{
    public class GlobalLoggerNotConfiguredForTestCorrelationExceptionTests
    {
        [Fact]
        public void A_GlobalLoggerNotConfiguredForTestCorrelationException_message_should_contain_the_problem()
        {
            new GlobalLoggerNotConfiguredForTestCorrelationException().Message
                .Should().Contain("Serilog's global logger has not been configured for test correlation.");
        }

        [Fact]
        public void A_GlobalLoggerNotConfiguredForTestCorrelationException_should_contain_the_consequence()
        {
            new GlobalLoggerNotConfiguredForTestCorrelationException().Message.Should().Contain(
                "The SerilogTestCorrelator will not be able to collect LogEvents.");
        }

        [Fact]
        public void A_GlobalLoggerNotConfiguredForTestCorrelationException_should_contain_the_reason()
        {
            new GlobalLoggerNotConfiguredForTestCorrelationException().Message.Should().Contain(
                "This may be because you did not call SerilogTestCorrelator.ConfigureGlobalLoggerForTestCorrelation(), or because other code has overwritten Serilog.Log.Logger since you did.");
        }
    }
}