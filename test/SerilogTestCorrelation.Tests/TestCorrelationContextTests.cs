using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Serilog;
using Serilog.Events;
using Xunit;

namespace SerilogTestCorrelation.Tests
{
    public partial class SerilogTestCorrelatorTests
    {
        [Fact]
        public void A_TestCorrelationContext_does_capture_LogEvents_inside_its_scope()
        {
            using (var context = TestCorrelator.CreateTestCorrelationContext())
            {
                Log.Information("");

                TestCorrelator.GetLogEventsFromTestCorrelationContext(context.Guid).Should().ContainSingle();
            }
        }

        [Fact]
        public void A_TestCorrelationContext_does_capture_LogEvents_inside_its_scope_even_in_extracted_methods()
        {
            using (var context = TestCorrelator.CreateTestCorrelationContext())
            {
                LogInformation();

                TestCorrelator.GetLogEventsFromTestCorrelationContext(context.Guid).Should().ContainSingle();
            }
        }

        static void LogInformation()
        {
            Log.Information("");
        }

        [Fact]
        public void A_TestCorrelationContext_does_not_capture_LogEvents_outside_its_scope()
        {
            Guid testCorrelationContextGuid;

            using (var context = TestCorrelator.CreateTestCorrelationContext())
            {
                testCorrelationContextGuid = context.Guid;
            }

            Log.Information("");

            TestCorrelator.GetLogEventsFromTestCorrelationContext(testCorrelationContextGuid).Should().BeEmpty();
        }

        [Fact]
        public void A_TestCorrelationContext_does_capture_LogEvents_inside_the_same_logical_call_context()
        {
            using (var context = TestCorrelator.CreateTestCorrelationContext())
            {
                var logTask = Task.Run(() => { Log.Information(""); });

                Task.WaitAll(logTask);

                TestCorrelator.GetLogEventsFromTestCorrelationContext(context.Guid).Should().ContainSingle();
            }
        }

        [Fact]
        public void
            A_TestCorrelationContext_does_capture_LogEvents_inside_the_same_logical_call_context_even_when_they_are_in_tasks_started_outside_of_it()
        {
            Task logTask;
            Guid testCorrelationContextGuid;

            using (var context = TestCorrelator.CreateTestCorrelationContext())
            {
                logTask = new Task(() => { Log.Information(""); });

                testCorrelationContextGuid = context.Guid;
            }

            logTask.Start();

            Task.WaitAll(logTask);

            TestCorrelator.GetLogEventsFromTestCorrelationContext(testCorrelationContextGuid).Should()
                .ContainSingle();
        }

        [Fact]
        public void
            A_TestCorrelationContext_does_not_capture_LogEvents_outside_the_same_logical_call_context_even_when_they_run_concurrently()
        {
            var usingEnteredSignal = new ManualResetEvent(false);

            var loggingFinishedSignal = new ManualResetEvent(false);

            var testCorrelationContextGuid = Guid.NewGuid();

            var logTask = Task.Run(() =>
            {
                usingEnteredSignal.WaitOne();

                Log.Information("");

                loggingFinishedSignal.Set();
            });

            var logContextTask = Task.Run(() =>
            {
                using (var context = TestCorrelator.CreateTestCorrelationContext())
                {
                    usingEnteredSignal.Set();
                    loggingFinishedSignal.WaitOne();
                    testCorrelationContextGuid = context.Guid;
                }
            });

            Task.WaitAll(logTask, logContextTask);

            TestCorrelator.GetLogEventsFromTestCorrelationContext(testCorrelationContextGuid).Should().BeEmpty();
        }

        [Fact]
        public void
            A_TestCorrelationContext_does_not_capture_LogEvents_outside_the_same_logical_call_context_even_when_they_are_in_tasks_started_inside_of_it()
        {
            var logTask = new Task(() => { Log.Information(""); });

            using (var context = TestCorrelator.CreateTestCorrelationContext())
            {
                logTask.Start();

                Task.WaitAll(logTask);

                TestCorrelator.GetLogEventsFromTestCorrelationContext(context.Guid).Should().BeEmpty();
            }
        }

        [Fact]
        public void
            A_TestCorrelationContext_within_a_TestCorrelationContext_adds_an_additional_TestCorrelationContext_to_LogEvents()
        {
            using (var outerContext = TestCorrelator.CreateTestCorrelationContext())
            {
                using (var innerContext = TestCorrelator.CreateTestCorrelationContext())
                {
                    Log.Information("");

                    TestCorrelator.GetLogEventsFromTestCorrelationContext(innerContext.Guid)
                        .Should()
                        .ContainSingle();

                    TestCorrelator.GetLogEventsFromTestCorrelationContext(outerContext.Guid)
                        .Should()
                        .ContainSingle();
                }
            }
        }

        [Fact]
        public void A_TestCorrelationContext_does_not_enrich_LogEvents_emitted_within_it()
        {
            using (var context = TestCorrelator.CreateTestCorrelationContext())
            {
                Log.Write(LogEventLevel.Information, "");

                TestCorrelator.GetLogEventsFromTestCorrelationContext(context.Guid)
                    .Should()
                    .ContainSingle()
                    .Which.Properties.Should()
                    .BeEmpty();
            }
        }
    }
}