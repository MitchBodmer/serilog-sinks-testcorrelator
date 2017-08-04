using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;

namespace SerilogTestCorrelation
{
    public static class LoggerAuditSinkConfigurationExtensions
    {
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