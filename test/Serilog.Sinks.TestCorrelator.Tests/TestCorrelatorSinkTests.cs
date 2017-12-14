using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Serilog.Sinks.TestCorrelator.Tests
{
    [TestClass]
    public class TestCorrelatorSinkTests
    {
        [TestMethod]
        public void A_TestCorrelatorSink_writes_LogEvents_emited_to_it_to_a_TestCorrelator()
        {
            var logger = new LoggerConfiguration().WriteTo.Sink(new TestCorrelatorSink()).CreateLogger();

            using (TestCorrelator.CreateContext())
            {
                logger.Information("");

                TestCorrelator.GetLogEventsFromCurrentContext().Should().ContainSingle();
            }
        }
    }
}
