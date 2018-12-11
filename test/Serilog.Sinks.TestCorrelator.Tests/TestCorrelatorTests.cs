using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serilog.Events;
using Serilog.Parsing;

namespace Serilog.Sinks.TestCorrelator.Tests
{
    [TestClass]
    public class TestCorrelatorTests
    {
        [AssemblyInitialize]
        public static void ConfigureGlobalLogger(TestContext testContext)
        {
            Log.Logger = new LoggerConfiguration().WriteTo.TestCorrelator().CreateLogger();
        }

        [TestMethod]
        public void A_context_captures_all_LogEvents_emitted_to_a_TestCorrelatorContext_within_it()
        {
            using (TestCorrelator.CreateContext())
            {
                Log.Information("");

                TestCorrelator.GetLogEventsFromCurrentContext()
                    .Should().ContainSingle();
            }
        }

        [TestMethod]
        public void A_context_captures_LogEvents_even_in_sub_methods()
        {
            using (TestCorrelator.CreateContext())
            {
                LogInformation();

                TestCorrelator.GetLogEventsFromCurrentContext()
                    .Should().ContainSingle();
            }
        }

        static void LogInformation()
        {
            Log.Information("");
        }

        [TestMethod]
        public void A_context_does_not_capture_LogEvents_outside_of_it()
        {
            Guid contextGuid;

            using (var context = TestCorrelator.CreateContext())
            {
                contextGuid = context.Guid;
            }

            Log.Information("");

            TestCorrelator.GetLogEventsFromContextGuid(contextGuid)
                .Should().BeEmpty();
        }

        [TestMethod]
        public void A_context_captures_LogEvents_inside_the_same_logical_call_context()
        {
            using (TestCorrelator.CreateContext())
            {
                var logTask = Task.Run(() => { Log.Information(""); });

                Task.WaitAll(logTask);

                TestCorrelator.GetLogEventsFromCurrentContext()
                    .Should().ContainSingle();
            }
        }

        [TestMethod]
        public void
            A_context_captures_LogEvents_inside_the_same_logical_call_context_even_when_they_are_in_tasks_started_outside_of_it()
        {
            Task logTask;
            Guid contextGuid;

            using (var context = TestCorrelator.CreateContext())
            {
                logTask = new Task(() => { Log.Information(""); });

                contextGuid = context.Guid;
            }

            logTask.Start();

            Task.WaitAll(logTask);

            TestCorrelator.GetLogEventsFromContextGuid(contextGuid)
                .Should().ContainSingle();
        }

        [TestMethod]
        public void
            A_context_does_not_capture_LogEvents_outside_the_same_logical_call_context_even_when_they_run_concurrently()
        {
            var usingEnteredSignal = new ManualResetEvent(false);

            var loggingFinishedSignal = new ManualResetEvent(false);

            var contextGuid = Guid.Empty;

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
                    contextGuid = context.Guid;
                }
            });

            Task.WaitAll(logTask, logContextTask);

            TestCorrelator.GetLogEventsFromContextGuid(contextGuid)
                .Should().BeEmpty();
        }

        [TestMethod]
        public void
            A_context_does_not_capture_LogEvents_outside_the_same_logical_call_context_even_when_they_are_in_tasks_started_inside_of_it()
        {
            var logTask = new Task(() => { Log.Information(""); });

            using (TestCorrelator.CreateContext())
            {
                logTask.Start();

                Task.WaitAll(logTask);

                TestCorrelator.GetLogEventsFromCurrentContext()
                    .Should().BeEmpty();
            }
        }

        [TestMethod]
        public void A_context_within_a_context_adds_an_additional_context_to_LogEvents()
        {
            using (var outerContext = TestCorrelator.CreateContext())
            {
                using (var innerContext = TestCorrelator.CreateContext())
                {
                    Log.Information("");

                    TestCorrelator.GetLogEventsFromContextGuid(innerContext.Guid)
                        .Should().ContainSingle();

                    TestCorrelator.GetLogEventsFromContextGuid(outerContext.Guid)
                        .Should().ContainSingle();
                }
            }
        }

        [TestMethod]
        public void Getting_LogEvents_from_the_current_context_gets_LogEvents_from_the_innermost_context()
        {
            using (TestCorrelator.CreateContext())
            {
                Log.Information("");

                using (TestCorrelator.CreateContext())
                {
                    Log.Information("");

                    TestCorrelator.GetLogEventsFromCurrentContext()
                        .Should().ContainSingle();
                }
            }
        }

        [TestMethod]
        public void Getting_LogEvents_from_the_current_context_gets_LogEvents_emitted_within_sub_contexts()
        {
            using (TestCorrelator.CreateContext())
            {
                Log.Information("");

                using (TestCorrelator.CreateContext())
                {
                    Log.Information("");
                }

                TestCorrelator.GetLogEventsFromCurrentContext()
                    .Should().HaveCount(2);
            }
        }

        [TestMethod]
        public void Getting_LogEvents_from_the_current_context_should_return_LogEvents_from_the_context_in_which_it_was_created_even_when_enumerated_outside_of_it()
        {
            IEnumerable<LogEvent> logEventsFromCurrentContext;
            
            using (TestCorrelator.CreateContext())
            {
                Log.Information("");
                logEventsFromCurrentContext = TestCorrelator.GetLogEventsFromCurrentContext();
                logEventsFromCurrentContext.Should().HaveCount(1);
            }

            Log.Information("");
            logEventsFromCurrentContext.Should().HaveCount(1);
        }

        [TestMethod]
        public void A_context_does_not_enrich_LogEvents_emitted_within_it()
        {
            using (TestCorrelator.CreateContext())
            {
                Log.Information("");

                TestCorrelator.GetLogEventsFromCurrentContext()
                    .Should().ContainSingle()
                    .Which.Properties.Should().BeEmpty();
            }
        }

        [TestMethod]
        public void Getting_LogEvents_preserves_the_order_in_which_they_were_emitted()
        {
            var logEvents =
                Enumerable.Range(0, 100)
                .Select(_ =>
                    new LogEvent(
                        DateTimeOffset.MinValue,
                        LogEventLevel.Information,
                        null,
                        new MessageTemplate(Enumerable.Empty<MessageTemplateToken>()),
                        Enumerable.Empty<LogEventProperty>()))
                .ToList();

            using (TestCorrelator.CreateContext())
            {
                foreach (var logEvent in logEvents)
                {
                    Log.Write(logEvent);
                }

                TestCorrelator.GetLogEventsFromCurrentContext()
                    .Should().Equal(logEvents);
            }
        }

        [TestMethod]
        public void The_LogEvent_stream_for_the_current_context_creates_notifications_for_LogEvents_emitted_within_the_current_context()
        {
            var scheduler = new TestScheduler();

            using (TestCorrelator.CreateContext())
            {
                scheduler.Schedule(TimeSpan.FromTicks(2), () => Log.Information(""));

                scheduler.Start(TestCorrelator.GetLogEventStreamFromCurrentContext, 0, 1, 3)
                    .Messages
                    .Should().ContainSingle()
                    .Which.Value.Kind.Should().Be(NotificationKind.OnNext);
            }
        }

        [TestMethod]
        public void The_LogEvent_stream_for_a_context_guid_creates_notifications_for_LogEvents_emitted_within_that_context()
        {
            var scheduler = new TestScheduler();

            using (var context = TestCorrelator.CreateContext())
            {
                scheduler.Schedule(TimeSpan.FromTicks(2), () => Log.Information(""));

                scheduler.Start(() => TestCorrelator.GetLogEventStreamFromContextGuid(context.Guid), 0, 1, 3)
                    .Messages
                    .Should().ContainSingle()
                    .Which.Value.Kind.Should().Be(NotificationKind.OnNext);
            }
        }

        [TestMethod]
        public void The_LogEvent_stream_for_the_current_context_does_not_create_notifications_for_LogEvents_emitted_outside_the_current_context()
        {
            var scheduler = new TestScheduler();

            scheduler.Schedule(TimeSpan.FromTicks(2), () => Log.Information(""));

            scheduler.Start(() =>
                    {
                        using (TestCorrelator.CreateContext())
                        {
                            return TestCorrelator.GetLogEventStreamFromCurrentContext();
                        }
                    }
                    , 0, 1, 3)
                .Messages
                .Should().BeEmpty();
        }
    }
}