using System;
using FluentAssertions;
using Xunit;

namespace Serilog.Utilities.ConcurrentCorrelator.Tests
{
    public partial class TestSerilogLogEventsTests
    {
        public TestSerilogLogEventsTests()
        {
            TestSerilogLogEvents.ConfigureGlobalLoggerForTestCorrelation();
        }

        [Fact]
        public void After_ConfigureGlobalLoggerForTestCorrelation_is_called_the_global_logger_is_a_Logger()
        {
            Log.Logger.Should().BeOfType<Core.Logger>();
        }

        [Fact]
        public void Calling_ConfigureGlobalLoggerForTestCorrelation_twice_does_not_clear_previously_collected_LogEvents()
        {
            using (var context = TestSerilogLogEvents.CreateTestCorrelationContext())
            {
                Log.Information("");

                TestSerilogLogEvents.ConfigureGlobalLoggerForTestCorrelation();

                TestSerilogLogEvents.GetLogEventsFromTestCorrelationContext(context.Guid).Should().ContainSingle();
            }
        }

        [Fact]
        public void Calling_ConfigureGlobalLoggerForTestCorrelation_is_idempotent()
        {
            var oldLogger = Log.Logger;

            TestSerilogLogEvents.ConfigureGlobalLoggerForTestCorrelation();

            Log.Logger.Should().Be(oldLogger);
        }

        [Fact]
        public void
            CreateTestCorrelationContext_throws_a_GlobalLoggerNotConfiguredForTestCorrelationException_if_the_global_logger_is_not_configured_for_testing
            ()
        {
            try
            {
                MisconfigureGlobalLoggerForTesting();

                Action throwingAction = () => TestSerilogLogEvents.CreateTestCorrelationContext();

                throwingAction.ShouldThrow<GlobalLoggerNotConfiguredForTestCorrelationException>();
            }
            finally
            {
                TestSerilogLogEvents.ConfigureGlobalLoggerForTestCorrelation();
            }
        }

        [Fact]
        public void
            GetLogEventsFromTestCorrelationContext_throws_a_GlobalLoggerNotConfiguredForTestCorrelationException_if_the_global_logger_is_not_configured_for_testing
            ()
        {
            try
            {
                MisconfigureGlobalLoggerForTesting();

                Action throwingAction =
                    () => TestSerilogLogEvents.GetLogEventsFromTestCorrelationContext(Guid.Empty);

                throwingAction.ShouldThrow<GlobalLoggerNotConfiguredForTestCorrelationException>();
            }
            finally
            {
                TestSerilogLogEvents.ConfigureGlobalLoggerForTestCorrelation();
            }
        }

        static void MisconfigureGlobalLoggerForTesting()
        {
            Log.Logger = new LoggerConfiguration().CreateLogger();
        }
    }
}