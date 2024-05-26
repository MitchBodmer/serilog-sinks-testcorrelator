using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks.TestCorrelator;

namespace Serilog;

/// <summary>
/// Adds the AuditTo.TestCorrelator extension method to a LoggerAuditSinkConfiguration.
/// </summary>
public static class LoggerAuditSinkConfigurationExtensions
{
    /// <summary>
    /// Adds a <see cref="TestCorrelatorSink"/> to audit to.
    /// </summary>
    /// <param name="loggerAuditSinkConfiguration">The LoggerAuditSinkConfiguration to audit to.</param>
    /// <param name="restrictedToMinimumLevel">The minimum LogEventLevel for events passed to the sink. Ignored when <paramref name="levelSwitch"/> is specified.</param>
    /// <param name="levelSwitch">A switch allowing the minimum LogEventLevel to be changed at runtime.</param>
    /// <returns>A modified LoggerConfiguration allowing configuration to continue.</returns>
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