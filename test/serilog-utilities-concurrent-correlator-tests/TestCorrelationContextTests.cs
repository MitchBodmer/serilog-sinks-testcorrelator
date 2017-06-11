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
        public void A_TestCorrelationContext_does_capture_LogEvents_inside_its_scope()
        {
            using (var context = TestSerilogLogEvents.CreateTestCorrelationContext())
            {
                Log.Information("");

                TestSerilogLogEvents.GetLogEventsFromTestCorrelationContext(context.Guid).Should().ContainSingle();
            }
        }

        [Fact]
        public void A_TestCorrelationContext_does_capture_LogEvents_inside_its_scope_even_in_extracted_methods()
        {
            using (var context = TestSerilogLogEvents.CreateTestCorrelationContext())
            {
                LogInformation();

                TestSerilogLogEvents.GetLogEventsFromTestCorrelationContext(context.Guid).Should().ContainSingle();
            }
        }

        static void LogInformation()
        {
            Log.Information("");
        }

        [Fact]
        public void A_TestCorrelationContext_does_not_capture_LogEvents_outside_its_scope()
        {
            Guid testCorrelationContextIdentifier;

            using (var context = TestSerilogLogEvents.CreateTestCorrelationContext())
            {
                testCorrelationContextIdentifier = context.Guid;
            }

            Log.Information("");

            TestSerilogLogEvents.GetLogEventsFromTestCorrelationContext(testCorrelationContextIdentifier).Should().BeEmpty();
        }

        [Fact]
        public void A_TestCorrelationContext_does_capture_LogEvents_inside_the_same_logical_call_context()
        {
            using (var context = TestSerilogLogEvents.CreateTestCorrelationContext())
            {
                var logTask = Task.Run(() => { Log.Information(""); });

                Task.WaitAll(logTask);

                TestSerilogLogEvents.GetLogEventsFromTestCorrelationContext(context.Guid).Should().ContainSingle();
            }
        }

        [Fact]
        public void
            A_TestCorrelationContext_does_capture_LogEvents_inside_the_same_logical_call_context_even_when_they_are_in_tasks_started_outside_of_it()
        {
            Task logTask;
            Guid testCorrelationContextIdentifier;

            using (var context = TestSerilogLogEvents.CreateTestCorrelationContext())
            {
                logTask = new Task(() => { Log.Information(""); });

                testCorrelationContextIdentifier = context.Guid;
            }

            logTask.Start();

            Task.WaitAll(logTask);

            TestSerilogLogEvents.GetLogEventsFromTestCorrelationContext(testCorrelationContextIdentifier).Should().ContainSingle();
        }

        [Fact]
        public void
            A_TestCorrelationContext_does_not_capture_LogEvents_outside_the_same_logical_call_context_even_when_they_run_concurrently()
        {
            var usingEnteredSignal = new ManualResetEvent(false);

            var loggingFinishedSignal = new ManualResetEvent(false);

            var testCorrelationContextIdentifier = Guid.NewGuid();

            var logTask = Task.Run(() =>
            {
                usingEnteredSignal.WaitOne();

                Log.Information("");

                loggingFinishedSignal.Set();
            });

            var logContextTask = Task.Run(() =>
            {
                using (var context = TestSerilogLogEvents.CreateTestCorrelationContext())
                {
                    usingEnteredSignal.Set();
                    loggingFinishedSignal.WaitOne();
                    testCorrelationContextIdentifier = context.Guid;
                }
            });

            Task.WaitAll(logTask, logContextTask);

            TestSerilogLogEvents.GetLogEventsFromTestCorrelationContext(testCorrelationContextIdentifier).Should().BeEmpty();
        }

        [Fact]
        public void
            A_TestCorrelationContext_does_not_capture_LogEvents_outside_the_same_logical_call_context_even_when_they_are_in_tasks_started_inside_of_it()
        {
            var logTask = new Task(() => { Log.Information(""); });

            using (var context = TestSerilogLogEvents.CreateTestCorrelationContext())
            {
                logTask.Start();

                Task.WaitAll(logTask);

                TestSerilogLogEvents.GetLogEventsFromTestCorrelationContext(context.Guid).Should().BeEmpty();
            }
        }

        [Fact]
        public void A_TestCorrelationContext_within_a_TestCorrelationContext_adds_an_additional_TestCorrelationContext_to_LogEvents()
        {
            using (var outerContext = TestSerilogLogEvents.CreateTestCorrelationContext())
            {
                using (var innerContext = TestSerilogLogEvents.CreateTestCorrelationContext())
                {
                    Log.Information("");

                    TestSerilogLogEvents.GetLogEventsFromTestCorrelationContext(innerContext.Guid)
                        .Should()
                        .ContainSingle();

                    TestSerilogLogEvents.GetLogEventsFromTestCorrelationContext(outerContext.Guid)
                        .Should()
                        .ContainSingle();
                }
            }
        }

        [Fact]
        public void A_TestCorrelationContext_does_not_enrich_LogEvents_emitted_within_it()
        {
            using (var context = TestSerilogLogEvents.CreateTestCorrelationContext())
            {
                Log.Write(LogEventLevel.Information, "");

                TestSerilogLogEvents.GetLogEventsFromTestCorrelationContext(context.Guid)
                    .Should()
                    .ContainSingle()
                    .Which.Properties.Should()
                    .BeEmpty();
            }
        }
    }
}