using System;
using FluentAssertions;
using Serilog;
using Xunit;

namespace SerilogTestCorrelation.Tests
{
    public partial class SerilogTestCorrelatorTests
    {
        public SerilogTestCorrelatorTests()
        {
            SerilogTestCorrelator.ConfigureGlobalLoggerForTestCorrelation();
        }

        [Fact]
        public void After_calling_ConfigureGlobalLoggerForTestCorrelation_the_global_logger_is_a_Logger()
        {
            Log.Logger.Should().BeOfType<Serilog.Core.Logger>();
        }

        [Fact]
        public void Calling_ConfigureGlobalLoggerForTestCorrelation_twice_does_not_clear_previously_collected_LogEvents()
        {
            using (var context = SerilogTestCorrelator.CreateTestCorrelationContext())
            {
                Log.Information("");

                SerilogTestCorrelator.ConfigureGlobalLoggerForTestCorrelation();

                SerilogTestCorrelator.GetLogEventsFromTestCorrelationContext(context.Guid).Should().ContainSingle();
            }
        }

        [Fact]
        public void Calling_ConfigureGlobalLoggerForTestCorrelation_is_idempotent()
        {
            var oldLogger = Log.Logger;

            SerilogTestCorrelator.ConfigureGlobalLoggerForTestCorrelation();

            Log.Logger.Should().Be(oldLogger);
        }

        [Fact]
        public void
            Calling_CreateTestCorrelationContext_throws_a_GlobalLoggerNotConfiguredForTestCorrelationException_if_the_global_logger_is_not_configured_for_test_correlation
            ()
        {
            try
            {
                MisconfigureGlobalLoggerForTestCorrelation();

                Action throwingAction = () => SerilogTestCorrelator.CreateTestCorrelationContext();

                throwingAction.ShouldThrow<GlobalLoggerNotConfiguredForTestCorrelationException>();
            }
            finally
            {
                SerilogTestCorrelator.ConfigureGlobalLoggerForTestCorrelation();
            }
        }

        [Fact]
        public void
            Calling_GetLogEventsFromTestCorrelationContext_throws_a_GlobalLoggerNotConfiguredForTestCorrelationException_if_the_global_logger_is_not_configured_for_test_correlation
            ()
        {
            try
            {
                MisconfigureGlobalLoggerForTestCorrelation();

                Action throwingAction =
                    () => SerilogTestCorrelator.GetLogEventsFromTestCorrelationContext(Guid.Empty);

                throwingAction.ShouldThrow<GlobalLoggerNotConfiguredForTestCorrelationException>();
            }
            finally
            {
                SerilogTestCorrelator.ConfigureGlobalLoggerForTestCorrelation();
            }
        }

        static void MisconfigureGlobalLoggerForTestCorrelation()
        {
            Log.Logger = new LoggerConfiguration().CreateLogger();
        }
    }
}