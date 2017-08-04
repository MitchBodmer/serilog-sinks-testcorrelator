using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;

namespace Serilog.Sinks.TestCorrelator
{
    public static class LoggerSinkConfigurationExtensions
    {
        public static LoggerConfiguration TestCorrelator(
            this LoggerSinkConfiguration loggerSettingsConfiguration,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            LoggingLevelSwitch levelSwitch = null)
        {
            return loggerSettingsConfiguration.Sink(
                new TestCorrelatorSink(),
                restrictedToMinimumLevel,
                levelSwitch);
        }
    }
}