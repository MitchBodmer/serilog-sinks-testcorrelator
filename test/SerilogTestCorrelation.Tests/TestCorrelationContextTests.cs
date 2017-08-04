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
            using (var context = TestCorrelator.CreateContext())
            {
                Log.Information("");

                TestCorrelator.GetLogEventsFromContext(context.Guid).Should().ContainSingle();
            }
        }

        [Fact]
        public void A_TestCorrelationContext_does_capture_LogEvents_inside_its_scope_even_in_extracted_methods()
        {
            using (var context = TestCorrelator.CreateContext())
            {
                LogInformation();

                TestCorrelator.GetLogEventsFromContext(context.Guid).Should().ContainSingle();
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

            using (var context = TestCorrelator.CreateContext())
            {
                testCorrelationContextGuid = context.Guid;
            }

            Log.Information("");

            TestCorrelator.GetLogEventsFromContext(testCorrelationContextGuid).Should().BeEmpty();
        }

        [Fact]
        public void A_TestCorrelationContext_does_capture_LogEvents_inside_the_same_logical_call_context()
        {
            using (var context = TestCorrelator.CreateContext())
            {
                var logTask = Task.Run(() => { Log.Information(""); });

                Task.WaitAll(logTask);

                TestCorrelator.GetLogEventsFromContext(context.Guid).Should().ContainSingle();
            }
        }

        [Fact]
        public void
            A_TestCorrelationContext_does_capture_LogEvents_inside_the_same_logical_call_context_even_when_they_are_in_tasks_started_outside_of_it()
        {
            Task logTask;
            Guid testCorrelationContextGuid;

            using (var context = TestCorrelator.CreateContext())
            {
                logTask = new Task(() => { Log.Information(""); });

                testCorrelationContextGuid = context.Guid;
            }

            logTask.Start();

            Task.WaitAll(logTask);

            TestCorrelator.GetLogEventsFromContext(testCorrelationContextGuid).Should()
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
                using (var context = TestCorrelator.CreateContext())
                {
                    usingEnteredSignal.Set();
                    loggingFinishedSignal.WaitOne();
                    testCorrelationContextGuid = context.Guid;
                }
            });

            Task.WaitAll(logTask, logContextTask);

            TestCorrelator.GetLogEventsFromContext(testCorrelationContextGuid).Should().BeEmpty();
        }

        [Fact]
        public void
            A_TestCorrelationContext_does_not_capture_LogEvents_outside_the_same_logical_call_context_even_when_they_are_in_tasks_started_inside_of_it()
        {
            var logTask = new Task(() => { Log.Information(""); });

            using (var context = TestCorrelator.CreateContext())
            {
                logTask.Start();

                Task.WaitAll(logTask);

                TestCorrelator.GetLogEventsFromContext(context.Guid).Should().BeEmpty();
            }
        }

        [Fact]
        public void
            A_TestCorrelationContext_within_a_TestCorrelationContext_adds_an_additional_TestCorrelationContext_to_LogEvents()
        {
            using (var outerContext = TestCorrelator.CreateContext())
            {
                using (var innerContext = TestCorrelator.CreateContext())
                {
                    Log.Information("");

                    TestCorrelator.GetLogEventsFromContext(innerContext.Guid)
                        .Should()
                        .ContainSingle();

                    TestCorrelator.GetLogEventsFromContext(outerContext.Guid)
                        .Should()
                        .ContainSingle();
                }
            }
        }

        [Fact]
        public void A_TestCorrelationContext_does_not_enrich_LogEvents_emitted_within_it()
        {
            using (var context = TestCorrelator.CreateContext())
            {
                Log.Write(LogEventLevel.Information, "");

                TestCorrelator.GetLogEventsFromContext(context.Guid)
                    .Should()
                    .ContainSingle()
                    .Which.Properties.Should()
                    .BeEmpty();
            }
        }
    }
}