using System;
using FluentAssertions;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Serilog.Fakes;
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
        [Test]
        [TestMethod]
        public void After_ConfigureGlobalLoggerForTesting_is_called_the_global_logger_is_a_Logger()
        {
            Log.Logger.Should().BeOfType<Core.Logger>();
        }

        [Fact]
        [Test]
        [TestMethod]
        public void Calling_ConfigureGlobalLoggerForTesting_twice_does_not_clear_previously_collected_LogEvents()
        {
            using (var context = TestSerilogLogEvents.EstablishTestLogContext())
            {
                Log.Information("");

                TestSerilogLogEvents.ConfigureGlobalLoggerForTesting();

                TestSerilogLogEvents.GetLogEventsWithContextIdentifier(context.Identifier).Should().ContainSingle();
            }
        }

        [Fact]
        [Test]
        [TestMethod]
        public void Calling_ConfigureGlobalLoggerForTesting_is_idempotent()
        {
            var oldLogger = Log.Logger;

            TestSerilogLogEvents.ConfigureGlobalLoggerForTesting();

            Log.Logger.Should().Be(oldLogger);
        }

        [Fact]
        [Test]
        [TestMethod]
        public void
            EstablishTestLogContext_throws_a_TestSerilogEventsNotConfiguredException_if_the_global_logger_is_not_configured_for_testing()
        {
            using (ShimsContext.Create())
            {
                SetGlobalLoggerToNull();

                Action throwingAction = () => TestSerilogLogEvents.EstablishTestLogContext();

                throwingAction.ShouldThrow<TestSerilogLogEvents.TestSerilogEventsNotConfiguredException>()
                    .WithMessage(
                        "The global logger has not been configured for testing. This can either be because you did not call TestSerilogEvents.ConfigureGlobalLoggerForTesting(), or because other code has overridden the global logger.");
            }
        }

        [Fact]
        [Test]
        [TestMethod]
        public void
            WithTestLogContextIdentifier_throws_a_TestSerilogEventsNotConfiguredException_if_the_global_logger_is_not_configured_for_testing()
        {
            using (ShimsContext.Create())
            using (var context = TestSerilogLogEvents.EstablishTestLogContext())
            {
                SetGlobalLoggerToNull();

                Action throwingAction = () => TestSerilogLogEvents.GetLogEventsWithContextIdentifier(context.Identifier);

                throwingAction.ShouldThrow<TestSerilogLogEvents.TestSerilogEventsNotConfiguredException>()
                    .WithMessage(
                        "The global logger has not been configured for testing. This can either be because you did not call TestSerilogEvents.ConfigureGlobalLoggerForTesting(), or because other code has overridden the global logger.");
            }
        }

        static void SetGlobalLoggerToNull()
        {
            ShimLog.LoggerGet = () => null;
        }
    }
}