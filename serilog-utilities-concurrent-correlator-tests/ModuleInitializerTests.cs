using System;
using System.Linq;
using FluentAssertions;
using Serilog.Events;
using Serilog.Parsing;
using Xunit;

namespace Serilog.Utilities.ConcurrentCorrelator.Tests
{
    public class ModuleInitializerTests
    {
        [Fact]
        public void When_the_module_is_loaded_the_global_logger_is_not_a_SilentLogger()
        {
            //Force the module to load.
            using (new CorrelationLogContext())
            {
                Log.Logger.GetType().FullName.Should().NotBe("Serilog.Core.Pipeline.SilentLogger");
            }
        }

        [Fact]
        public void When_the_module_is_loaded_the_global_logger_is_a_Logger()
        {
            //Force the module to load.
            using (new CorrelationLogContext())
            {
                Log.Logger.GetType().FullName.Should().Be("Serilog.Core.Logger");
            }
        }

        [Theory]
        [InlineData(LogEventLevel.Information)]
        [InlineData(LogEventLevel.Debug)]
        [InlineData(LogEventLevel.Error)]
        [InlineData(LogEventLevel.Fatal)]
        [InlineData(LogEventLevel.Verbose)]
        [InlineData(LogEventLevel.Warning)]
        public void When_the_module_is_loaded_the_static_SerilogLogEvents_bag_receives_logs_of_all_LogEventLevels(LogEventLevel level)
        {
            //Force the module to load.
            using (new CorrelationLogContext())
            {
                var uniqueMessageTemplate = Guid.NewGuid().ToString();

                Log.Logger.Write(new LogEvent(DateTimeOffset.Now, level, null,
                    new MessageTemplate(uniqueMessageTemplate, Enumerable.Empty<MessageTemplateToken>()),
                    Enumerable.Empty<LogEventProperty>()));

                SerilogLogEvents.Bag.Should().Contain(logEvent => logEvent.MessageTemplate.Text == uniqueMessageTemplate);
            }
        }
    }
}