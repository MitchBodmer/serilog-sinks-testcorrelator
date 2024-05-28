using System.Collections.Generic;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks.TestCorrelator;

namespace Serilog;

/// <summary>
/// Adds the AuditTo.TestCorrelator extension methods to <seealso cref="LoggerAuditSinkConfiguration"/>.
/// </summary>
public static class LoggerAuditSinkConfigurationExtensions
{
    /// <summary>
    /// Audits <seealso cref="LogEvent"/>s to a <see cref="TestCorrelatorSink"/>.
    /// </summary>
    /// <param name="loggerAuditSinkConfiguration">The <seealso cref="LoggerAuditSinkConfiguration"/> to audit to.</param>
    /// <param name="ids">The <seealso cref="TestCorrelatorSinkId"/>s to apply to this sink.</param>
    /// <returns>A modified <seealso cref="LoggerConfiguration"/> allowing configuration to continue.</returns>
    public static LoggerConfiguration TestCorrelator(
        this LoggerAuditSinkConfiguration loggerAuditSinkConfiguration,
        params TestCorrelatorSinkId[] ids) =>
        loggerAuditSinkConfiguration.Sink(new TestCorrelatorSink(ids));

    /// <summary>
    /// Audits <seealso cref="LogEvent"/>s to a <see cref="TestCorrelatorSink"/>.
    /// </summary>
    /// <param name="loggerAuditSinkConfiguration">The <seealso cref="LoggerAuditSinkConfiguration"/> to audit to.</param>
    /// <param name="restrictedToMinimumLevel">The minimum <seealso cref="LogEventLevel"/> for events passed to the sink.</param>
    /// <param name="ids">The <seealso cref="TestCorrelatorSinkId"/>s to apply to this sink.</param>
    /// <returns>A modified <seealso cref="LoggerConfiguration"/> allowing configuration to continue.</returns>
    public static LoggerConfiguration TestCorrelator(
        this LoggerAuditSinkConfiguration loggerAuditSinkConfiguration,
        LogEventLevel restrictedToMinimumLevel,
        params TestCorrelatorSinkId[] ids) =>
        loggerAuditSinkConfiguration.Sink(
            new TestCorrelatorSink(ids),
            restrictedToMinimumLevel);

    /// <summary>
    /// Audits <seealso cref="LogEvent"/>s to a <see cref="TestCorrelatorSink"/>.
    /// </summary>
    /// <param name="loggerAuditSinkConfiguration">The <seealso cref="LoggerAuditSinkConfiguration"/> to audit to.</param>
    /// <param name="levelSwitch">A <seealso cref="LoggingLevelSwitch"/> allowing the minimum <seealso cref="LogEventLevel"/> to be changed at runtime.</param>
    /// <param name="ids">The <seealso cref="TestCorrelatorSinkId"/>s to apply to this sink.</param>
    /// <returns>A modified <seealso cref="LoggerConfiguration"/> allowing configuration to continue.</returns>
    public static LoggerConfiguration TestCorrelator(
        this LoggerAuditSinkConfiguration loggerAuditSinkConfiguration,
        LoggingLevelSwitch levelSwitch,
        params TestCorrelatorSinkId[] ids) =>
        loggerAuditSinkConfiguration.Sink(
            new TestCorrelatorSink(ids),
            LevelAlias.Minimum,
            levelSwitch);

    /// <summary>
    /// Audits <seealso cref="LogEvent"/>s to a <see cref="TestCorrelatorSink"/>.
    /// </summary>
    /// <param name="loggerAuditSinkConfiguration">The <seealso cref="LoggerAuditSinkConfiguration"/> to audit to.</param>
    /// <param name="ids">The <seealso cref="TestCorrelatorSinkId"/>s to apply to this sink.</param>
    /// <returns>A modified <seealso cref="LoggerConfiguration"/> allowing configuration to continue.</returns>
    public static LoggerConfiguration TestCorrelator(
        this LoggerAuditSinkConfiguration loggerAuditSinkConfiguration,
        IEnumerable<TestCorrelatorSinkId> ids) =>
        loggerAuditSinkConfiguration.Sink(new TestCorrelatorSink(ids));

    /// <summary>
    /// Audits <seealso cref="LogEvent"/>s to a <see cref="TestCorrelatorSink"/>.
    /// </summary>
    /// <param name="loggerAuditSinkConfiguration">The <seealso cref="LoggerAuditSinkConfiguration"/> to audit to.</param>
    /// <param name="restrictedToMinimumLevel">The minimum <seealso cref="LogEventLevel"/> for events passed to the sink.</param>
    /// <param name="ids">The <seealso cref="TestCorrelatorSinkId"/>s to apply to this sink.</param>
    /// <returns>A modified <seealso cref="LoggerConfiguration"/> allowing configuration to continue.</returns>
    public static LoggerConfiguration TestCorrelator(
        this LoggerAuditSinkConfiguration loggerAuditSinkConfiguration,
        LogEventLevel restrictedToMinimumLevel,
        IEnumerable<TestCorrelatorSinkId> ids) =>
        loggerAuditSinkConfiguration.Sink(
            new TestCorrelatorSink(ids),
            restrictedToMinimumLevel);

    /// <summary>
    /// Audits <seealso cref="LogEvent"/>s to a <see cref="TestCorrelatorSink"/>.
    /// </summary>
    /// <param name="loggerAuditSinkConfiguration">The <seealso cref="LoggerAuditSinkConfiguration"/> to audit to.</param>
    /// <param name="levelSwitch">A <seealso cref="LoggingLevelSwitch"/> allowing the minimum <seealso cref="LogEventLevel"/> to be changed at runtime.</param>
    /// <param name="ids">The <seealso cref="TestCorrelatorSinkId"/>s to apply to this sink.</param>
    /// <returns>A modified <seealso cref="LoggerConfiguration"/> allowing configuration to continue.</returns>
    public static LoggerConfiguration TestCorrelator(
        this LoggerAuditSinkConfiguration loggerAuditSinkConfiguration,
        LoggingLevelSwitch levelSwitch,
        IEnumerable<TestCorrelatorSinkId> ids) =>
        loggerAuditSinkConfiguration.Sink(
            new TestCorrelatorSink(ids),
            LevelAlias.Minimum,
            levelSwitch);
}