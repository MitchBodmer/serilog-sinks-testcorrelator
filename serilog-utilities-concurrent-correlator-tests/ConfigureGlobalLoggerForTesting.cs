﻿using System;
using FluentAssertions;
using Xunit;

namespace Serilog.Utilities.ConcurrentCorrelator.Tests
{
    public partial class TestSerilogLogEventsTests
    {
        public TestSerilogLogEventsTests()
        {
            TestSerilogLogEvents.ConfigureGlobalLoggerForTesting();
        }

        [Fact]
        public void After_ConfigureGlobalLoggerForTesting_is_called_the_global_logger_is_a_Logger()
        {
            Log.Logger.Should().BeOfType<Core.Logger>();
        }

        [Fact]
        public void Calling_ConfigureGlobalLoggerForTesting_twice_does_not_clear_previously_collected_LogEvents()
        {
            using (var context = TestSerilogLogEvents.EstablishTestLogContext())
            {
                Log.Information("");

                TestSerilogLogEvents.ConfigureGlobalLoggerForTesting();

                TestSerilogLogEvents.GetLogEventsWithContextIdentifier(context.Guid).Should().ContainSingle();
            }
        }

        [Fact]
        public void Calling_ConfigureGlobalLoggerForTesting_is_idempotent()
        {
            var oldLogger = Log.Logger;

            TestSerilogLogEvents.ConfigureGlobalLoggerForTesting();

            Log.Logger.Should().Be(oldLogger);
        }

        [Fact]
        public void
            EstablishTestLogContext_throws_a_TestSerilogEventsNotConfiguredException_if_the_global_logger_is_not_configured_for_testing()
        {
            try
            {
                MisconfigureGlobalLoggerForTesting();

                Action throwingAction = () => TestSerilogLogEvents.EstablishTestLogContext();

                throwingAction.ShouldThrow<TestSerilogEventsNotConfiguredException>()
                    .WithMessage(
                        "The global logger has not been configured for testing. This can either be because you did not call ConfigureGlobalLoggerForTesting, or because other code has overridden the global logger.");
            }
            finally
            {
                TestSerilogLogEvents.ConfigureGlobalLoggerForTesting();
            }
        }

        [Fact]
        public void
            WithTestLogContextIdentifier_throws_a_TestSerilogEventsNotConfiguredException_if_the_global_logger_is_not_configured_for_testing()
        {
            try
            {
                using (var context = TestSerilogLogEvents.EstablishTestLogContext())
                {
                    MisconfigureGlobalLoggerForTesting();

                    Action throwingAction =
                        () => TestSerilogLogEvents.GetLogEventsWithContextIdentifier(context.Guid);

                    throwingAction.ShouldThrow<TestSerilogEventsNotConfiguredException>()
                        .WithMessage(
                            "The global logger has not been configured for testing. This can either be because you did not call ConfigureGlobalLoggerForTesting, or because other code has overridden the global logger.");
                }
            }
            finally
            {
                TestSerilogLogEvents.ConfigureGlobalLoggerForTesting();
            }
        }

        static void MisconfigureGlobalLoggerForTesting()
        {
            Log.Logger = new LoggerConfiguration().CreateLogger();
        }
    }
}