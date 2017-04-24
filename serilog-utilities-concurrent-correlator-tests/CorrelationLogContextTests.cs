using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Xunit;

namespace Serilog.Utilities.ConcurrentCorrelator.Tests
{
    [TestClass]
    public class TestLogContextTests
    {
        public TestLogContextTests()
        {
            TestSerilogLogEvents.ConfigureGlobalLoggerForTesting();
        }

        [Fact]
        [Test]
        [TestMethod]
        public void A_TestLogContext_does_enrich_LogEvents_inside_its_scope()
        {
            using (var correlationLogContext = TestSerilogLogEvents.EstablishTestLogContext())
            {
                Log.Information("");

                TestSerilogLogEvents.WithTestLogContextGuid(correlationLogContext.Guid).Should().ContainSingle();
            }
        }

        [Fact]
        [Test]
        [TestMethod]
        public void A_TestLogContext_does_enrich_LogEvents_inside_its_scope_even_in_extracted_methods()
        {
            using (var correlationLogContext = TestSerilogLogEvents.EstablishTestLogContext())
            {
                LogInformation();

                TestSerilogLogEvents.WithTestLogContextGuid(correlationLogContext.Guid).Should().ContainSingle();
            }
        }

        static void LogInformation()
        {
            Log.Information("");
        }

        [Fact]
        [Test]
        [TestMethod]
        public void A_TestLogContext_does_not_enrich_LogEvents_outside_its_scope()
        {
            Guid correlationLogContextGuid;

            using (var correlationLogContext = TestSerilogLogEvents.EstablishTestLogContext())
            {
                correlationLogContextGuid = correlationLogContext.Guid;
            }

            Log.Information("");

            TestSerilogLogEvents.WithTestLogContextGuid(correlationLogContextGuid).Should().BeEmpty();
        }

        [Fact]
        [Test]
        [TestMethod]
        public void A_TestLogContext_does_enrich_LogEvents_inside_the_same_logical_call_context()
        {
            using (var context = TestSerilogLogEvents.EstablishTestLogContext())
            {
                var logTask = Task.Run(() =>
                {
                    Log.Information("");
                });

                Task.WaitAll(logTask);

                TestSerilogLogEvents.WithTestLogContextGuid(context.Guid).Should().ContainSingle();
            }
        }

        [Fact]
        [Test]
        [TestMethod]
        public void A_TestLogContext_does_enrich_LogEvents_inside_the_same_logical_call_context_even_when_they_are_in_tasks_started_outside_of_it()
        {
            Task logTask;
            Guid guid;

            using (var context = TestSerilogLogEvents.EstablishTestLogContext())
            {
                logTask = new Task(() =>
                {
                    Log.Information("");
                });

                guid = context.Guid;
            }

            logTask.Start();

            Task.WaitAll(logTask);

            TestSerilogLogEvents.WithTestLogContextGuid(guid).Should().ContainSingle();
        }

        [Fact]
        [Test]
        [TestMethod]
        public void A_TestLogContext_does_not_enrich_LogEvents_outside_the_same_logical_call_context()
        {
            var usingEnteredSignal = new ManualResetEvent(false);

            var loggingFinishedSignal = new ManualResetEvent(false);

            var logTask = Task.Run(() =>
            {
                usingEnteredSignal.WaitOne();

                Log.Information("");

                loggingFinishedSignal.Set();
            });

            var logContextTask = Task.Run(() =>
            {
                using (var context = TestSerilogLogEvents.EstablishTestLogContext())
                {
                    usingEnteredSignal.Set();
                    loggingFinishedSignal.WaitOne();
                    return context.Guid;
                }
            });

            Task.WaitAll(logTask, logContextTask);

            TestSerilogLogEvents.WithTestLogContextGuid(logContextTask.Result).Should().BeEmpty();
        }

        [Fact]
        [Test]
        [TestMethod]
        public void A_TestLogContext_does_not_enrich_LogEvents_outside_the_same_logical_call_context_even_when_they_are_in_tasks_started_inside_of_it()
        {
            var logTask = new Task(() =>
            {
                Log.Information("");
            });

            using (var context = TestSerilogLogEvents.EstablishTestLogContext())
            {
                logTask.Start();

                Task.WaitAll(logTask);

                TestSerilogLogEvents.WithTestLogContextGuid(context.Guid).Should().BeEmpty();
            }
        }

        [Fact]
        [Test]
        [TestMethod]
        public void A_TestLogContext_within_a_TestLogContext_adds_an_additional_TestLogContext_to_LogEvents()
        {
            using (var outerTestLogContext = TestSerilogLogEvents.EstablishTestLogContext())
            {
                using (var innerTestLogContext = TestSerilogLogEvents.EstablishTestLogContext())
                {
                    Log.Information("");

                    TestSerilogLogEvents.WithTestLogContextGuid(innerTestLogContext.Guid).Should().ContainSingle();

                    TestSerilogLogEvents.WithTestLogContextGuid(outerTestLogContext.Guid).Should().ContainSingle();
                }
            }
        }
    }
}
