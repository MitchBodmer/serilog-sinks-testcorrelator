using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks.TestCorrelator;

namespace Serilog
{
    /// <summary>
    /// Adds the <see cref="LoggerConfiguration.WriteTo"/>.<see cref="TestCorrelator"/>() extension method to <see cref="LoggerSinkConfiguration"/>.
    /// </summary>
    public static class LoggerSinkConfigurationExtensions
    {
        /// <summary>
        /// Adds a <see cref="TestCorrelatorSink"/> to write to.
        /// </summary>
        /// <param name="loggerSinkConfiguration">The <see cref="LoggerSinkConfiguration"/> to write to.</param>
        /// <param name="restrictedToMinimumLevel">The minimum <see cref="LogEventLevel"/> for events passed to the sink. Ignored when <paramref name="levelSwitch"/> is specified.</param>
        /// <param name="levelSwitch">A switch allowing the minimum <see cref="LogEventLevel"/> to be changed at runtime.</param>
        /// <returns>A modified <see cref="LoggerConfiguration"/> allowing configuration to continue.</returns>
        public static LoggerConfiguration TestCorrelator(
            this LoggerSinkConfiguration loggerSinkConfiguration,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            LoggingLevelSwitch levelSwitch = null)
        {
            return loggerSinkConfiguration.Sink(
                new TestCorrelatorSink(),
                restrictedToMinimumLevel,
                levelSwitch);
        }
    }
}