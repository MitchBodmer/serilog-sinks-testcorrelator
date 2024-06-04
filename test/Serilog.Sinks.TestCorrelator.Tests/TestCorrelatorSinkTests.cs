using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serilog.Events;

namespace Serilog.Sinks.TestCorrelator.Tests;

[TestClass]
public class TestCorrelatorSinkTests
{
    [TestMethod]
    public void A_TestCorrelatorSink_writes_LogEvents_emitted_to_it_to_a_TestCorrelator()
    {
        LogEvent logEvent = new(
            DateTimeOffset.UtcNow,
            LogEventLevel.Information,
            null,
            new MessageTemplate([]),
            []);

        using (TestCorrelator.CreateContext())
        {
            new TestCorrelatorSink().Emit(logEvent);

            TestCorrelator.GetLogEventsFromCurrentContext().Should().ContainSingle().Which.Should().Be(logEvent);
        }
    }
}