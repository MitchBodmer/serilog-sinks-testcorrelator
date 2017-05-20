using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using FluentAssertions;
using Xunit;

namespace Serilog.Utilities.ConcurrentCorrelator.Tests
{
    public class TestSerilogEventsNotConfiguredExceptionTests
    {
        [Fact]
        public void A_TestSerilogEventsNotConfiguredException_should_have_the_correct_message()
        {
            new TestSerilogEventsNotConfiguredException().Message.Should().Be("Serilog's global logger has not been configured for testing. This can either be because you did not call ConfigureGlobalLoggerForTesting, or because other code has overwritten Logger since you did.");
        }
    }
}
