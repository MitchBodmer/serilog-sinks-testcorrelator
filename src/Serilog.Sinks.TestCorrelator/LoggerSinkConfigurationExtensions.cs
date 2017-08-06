using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks.TestCorrelator;

namespace Serilog
{
    /// <summary>
    /// 
    /// </summary>
    public static class LoggerSinkConfigurationExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="loggerSettingsConfiguration"></param>
        /// <param name="restrictedToMinimumLevel"></param>
        /// <param name="levelSwitch"></param>
        /// <returns></returns>
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