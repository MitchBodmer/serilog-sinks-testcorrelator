using System;
using System.Collections.Generic;
using FluentAssertions;
using Serilog.Events;
using Xunit;

namespace Serilog.Utilities.ConcurrentCorrelator.Tests
{
    public class EnumerableOfLogEventExtensionsTests
    {
        [Fact]
        public void WithCorrelationLogContextGuid_returns_empty_if_no_log_events_have_been_logged()
        {
            new List<LogEvent>().WithCorrelationLogContextGuid(Guid.NewGuid()).Should().BeEmpty();
        }
    }
}
