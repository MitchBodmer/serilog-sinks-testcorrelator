using FluentAssertions;
using Serilog;
using Serilog.Context;
using Serilog.Events;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Serilog.Sinks.TestCorrelator;
using Xunit;

namespace SerilogTestCorrelation.Tests
{
    public class TestCorrelatorTests
    {
        public TestCorrelatorTests()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.TestCorrelator()
                .Enrich.FromLogContext()
                .CreateLogger();
        }

        [Fact]
        public void TestCorrelator_allows_you_to_filter_to_LogEvents_emitted_within_a_context()
        {
            Log.Error("");

            using (TestCorrelator.CreateContext())
            {
                Log.Error("");
            }

            Guid testCorrelationContextGuid;

            using (var context = TestCorrelator.CreateContext())
            {
                Log.Information("");

                testCorrelationContextGuid = context.Guid;
            }

            TestCorrelator.GetLogEventsFromContext(testCorrelationContextGuid)
                .Should().ContainSingle().Which.Level.Should().Be(LogEventLevel.Information);
        }

        [Theory]
        [InlineData(LogEventLevel.Information)]
        [InlineData(LogEventLevel.Debug)]
        [InlineData(LogEventLevel.Error)]
        [InlineData(LogEventLevel.Fatal)]
        [InlineData(LogEventLevel.Verbose)]
        [InlineData(LogEventLevel.Warning)]
        public void TestCorrelator_receives_LogEvents_of_all_LogEventLevels(LogEventLevel logEventLevel)
        {
            using (var context = TestCorrelator.CreateContext())
            {
                Log.Write(logEventLevel, "");

                TestCorrelator.GetLogEventsFromContext(context.Guid).Should().ContainSingle();
            }
        }

        [Fact]
        public void TestCorrelator_enriches_LogEvents_with_LogContext()
        {
            using (var context = TestCorrelator.CreateContext())
            {
                const string propertyName = "Property name";

                using (LogContext.PushProperty(propertyName, new object()))
                {
                    Log.Information("");
                }

                TestCorrelator.GetLogEventsFromContext(context.Guid)
                    .Should().ContainSingle().Which.Properties.Keys
                    .Should().ContainSingle().Which.Should().Be(propertyName);
            }
        }

        [Fact]
        public void GetLogEventsFromTestCorrelationContext_returns_empty_if_no_LogEvents_have_been_emitted()
        {
            using (var context = TestCorrelator.CreateContext())
            {
                TestCorrelator.GetLogEventsFromContext(context.Guid).Should().BeEmpty();
            }
        }

        [Fact]
        public void
            GetLogEventsFromTestCorrelationContext_returns_empty_if_no_LogEvents_have_been_emitted_within_the_context()
        {
            Log.Information("");

            using (var context = TestCorrelator.CreateContext())
            {
                TestCorrelator.GetLogEventsFromContext(context.Guid).Should().BeEmpty();
            }
        }

        [Fact]
        public void
            GetLogEventsFromTestCorrelationContext_returns_all_LogEvents_that_have_been_emitted_within_the_context()
        {
            using (var context = TestCorrelator.CreateContext())
            {
                const int expectedCount = 4;

                foreach (var unused in Enumerable.Range(0, expectedCount))
                {
                    Log.Information("");
                }

                TestCorrelator.GetLogEventsFromContext(context.Guid)
                    .Should().HaveCount(expectedCount);
            }
        }

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