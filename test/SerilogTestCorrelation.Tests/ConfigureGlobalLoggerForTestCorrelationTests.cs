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
        public void
            Calling_ConfigureGlobalLoggerForTestCorrelation_twice_does_not_clear_previously_collected_LogEvents()
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

        [Fact]
        public void A_GlobalLoggerNotConfiguredForTestCorrelationException_message_should_contain_the_problem()
        {
            try
            {
                MisconfigureGlobalLoggerForTestCorrelation();

                SerilogTestCorrelator.CreateTestCorrelationContext();
            }
            catch (GlobalLoggerNotConfiguredForTestCorrelationException exception)
            {
                exception.Message.Should()
                    .Contain("Serilog's global logger has not been configured for test correlation.");
            }
            finally
            {
                SerilogTestCorrelator.ConfigureGlobalLoggerForTestCorrelation();
            }
        }

        [Fact]
        public void A_GlobalLoggerNotConfiguredForTestCorrelationException_should_contain_the_consequence()
        {
            try
            {
                MisconfigureGlobalLoggerForTestCorrelation();

                SerilogTestCorrelator.CreateTestCorrelationContext();
            }
            catch (GlobalLoggerNotConfiguredForTestCorrelationException exception)
            {
                exception.Message.Should().Contain("The SerilogTestCorrelator will not be able to collect LogEvents.");
            }
            finally
            {
                SerilogTestCorrelator.ConfigureGlobalLoggerForTestCorrelation();
            }
        }

        [Fact]
        public void A_GlobalLoggerNotConfiguredForTestCorrelationException_should_contain_the_reason()
        {
            try
            {
                MisconfigureGlobalLoggerForTestCorrelation();

                SerilogTestCorrelator.CreateTestCorrelationContext();
            }
            catch (GlobalLoggerNotConfiguredForTestCorrelationException exception)
            {
                exception.Message.Should()
                    .Contain(
                        "This may be because you did not call SerilogTestCorrelator.ConfigureGlobalLoggerForTestCorrelation(), or because other code has overwritten Serilog.Log.Logger since you did.");
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