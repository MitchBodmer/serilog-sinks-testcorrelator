using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks.TestCorrelator;

namespace Serilog
{
    /// <summary>
    /// 
    /// </summary>
    public static class LoggerAuditSinkConfigurationExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="loggerAuditSinkConfiguration"></param>
        /// <param name="restrictedToMinimumLevel"></param>
        /// <param name="levelSwitch"></param>
        /// <returns></returns>
        public static LoggerConfiguration TestCorrelator(
            this LoggerAuditSinkConfiguration loggerAuditSinkConfiguration,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            LoggingLevelSwitch levelSwitch = null)
        {
            return loggerAuditSinkConfiguration.Sink(
                new TestCorrelatorSink(),
                restrictedToMinimumLevel,
                levelSwitch);
        }
    }
}