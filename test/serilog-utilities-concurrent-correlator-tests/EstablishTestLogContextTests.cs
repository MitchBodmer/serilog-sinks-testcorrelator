using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Serilog.Events;
using Xunit;

namespace Serilog.Utilities.ConcurrentCorrelator.Tests
{
    public partial class TestSerilogLogEventsTests
    {
        [Fact]
        public void A_TestLogContext_does_capture_LogEvents_inside_its_scope()
        {
            using (var context = TestSerilogLogEvents.EstablishTestLogContext())
            {
                Log.Information("");

                TestSerilogLogEvents.GetLogEventsWithContextIdentifier(context.Guid).Should().ContainSingle();
            }
        }

        [Fact]
        public void A_TestLogContext_does_capture_LogEvents_inside_its_scope_even_in_extracted_methods()
        {
            using (var context = TestSerilogLogEvents.EstablishTestLogContext())
            {
                LogInformation();

                TestSerilogLogEvents.GetLogEventsWithContextIdentifier(context.Guid).Should().ContainSingle();
            }
        }

        static void LogInformation()
        {
            Log.Information("");
        }

        [Fact]
        public void A_TestLogContext_does_not_capture_LogEvents_outside_its_scope()
        {
            Guid testLogContextIdentifier;

            using (var context = TestSerilogLogEvents.EstablishTestLogContext())
            {
                testLogContextIdentifier = context.Guid;
            }

            Log.Information("");

            TestSerilogLogEvents.GetLogEventsWithContextIdentifier(testLogContextIdentifier).Should().BeEmpty();
        }

        [Fact]
        public void A_TestLogContext_does_capture_LogEvents_inside_the_same_logical_call_context()
        {
            using (var context = TestSerilogLogEvents.EstablishTestLogContext())
            {
                var logTask = Task.Run(() => { Log.Information(""); });

                Task.WaitAll(logTask);

                TestSerilogLogEvents.GetLogEventsWithContextIdentifier(context.Guid).Should().ContainSingle();
            }
        }

        [Fact]
        public void
            A_TestLogContext_does_capture_LogEvents_inside_the_same_logical_call_context_even_when_they_are_in_tasks_started_outside_of_it()
        {
            Task logTask;
            Guid testLogContextIdentifier;

            using (var context = TestSerilogLogEvents.EstablishTestLogContext())
            {
                logTask = new Task(() => { Log.Information(""); });

                testLogContextIdentifier = context.Guid;
            }

            logTask.Start();

            Task.WaitAll(logTask);

            TestSerilogLogEvents.GetLogEventsWithContextIdentifier(testLogContextIdentifier).Should().ContainSingle();
        }

        [Fact]
        public void
            A_TestLogContext_does_not_capture_LogEvents_outside_the_same_logical_call_context_even_when_they_run_concurrently()
        {
            var usingEnteredSignal = new ManualResetEvent(false);

            var loggingFinishedSignal = new ManualResetEvent(false);

            var testLogContextIdentifier = Guid.NewGuid();

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
                    testLogContextIdentifier = context.Guid;
                }
            });

            Task.WaitAll(logTask, logContextTask);

            TestSerilogLogEvents.GetLogEventsWithContextIdentifier(testLogContextIdentifier).Should().BeEmpty();
        }

        [Fact]
        public void
            A_TestLogContext_does_not_capture_LogEvents_outside_the_same_logical_call_context_even_when_they_are_in_tasks_started_inside_of_it()
        {
            var logTask = new Task(() => { Log.Information(""); });

            using (var context = TestSerilogLogEvents.EstablishTestLogContext())
            {
                logTask.Start();

                Task.WaitAll(logTask);

                TestSerilogLogEvents.GetLogEventsWithContextIdentifier(context.Guid).Should().BeEmpty();
            }
        }

        [Fact]
        public void A_TestLogContext_within_a_TestLogContext_adds_an_additional_TestLogContext_to_LogEvents()
        {
            using (var outerContext = TestSerilogLogEvents.EstablishTestLogContext())
            {
                using (var innerContext = TestSerilogLogEvents.EstablishTestLogContext())
                {
                    Log.Information("");

                    TestSerilogLogEvents.GetLogEventsWithContextIdentifier(innerContext.Guid)
                        .Should()
                        .ContainSingle();

                    TestSerilogLogEvents.GetLogEventsWithContextIdentifier(outerContext.Guid)
                        .Should()
                        .ContainSingle();
                }
            }
        }

        [Fact]
        public void A_TestLogContext_does_not_enrich_LogEvents_emitted_within_it()
        {
            using (var context = TestSerilogLogEvents.EstablishTestLogContext())
            {
                Log.Write(LogEventLevel.Information, "");

                TestSerilogLogEvents.GetLogEventsWithContextIdentifier(context.Guid)
                    .Should()
                    .ContainSingle()
                    .Which.Properties.Should()
                    .BeEmpty();
            }
        }
    }
}