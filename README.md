# Serilog.Sinks.TestCorrelator [![AppVeyor Badge](https://ci.appveyor.com/api/projects/status/rjdxaaq2ry50v30c/branch/master?svg=true)](https://ci.appveyor.com/project/MitchBodmer/serilog-sinks-testcorrelator/branch/master) [![NuGet Badge](https://buildstats.info/nuget/Serilog.Sinks.TestCorrelator)](https://www.nuget.org/packages/Serilog.Sinks.TestCorrelator/)

A [Serilog](https://github.com/serilog/serilog) sink that correlates log events with the code that produced them, enabling unit testing of log output.

## Usage

Just create a logger that writes or audits to the TestCorrelator.

```csharp
Log.Logger = new LoggerConfiguration().WriteTo.TestCorrelator().CreateLogger();
```

Then wrap the code that you would like to monitor with a context and get the log events emitted within that context using the TestCorrelator.

```csharp
using (TestCorrelator.CreateContext())
{
    Log.Information("My log message!");

    TestCorrelator.GetLogEventsFromCurrentContext()
        .Should().ContainSingle()
        .Which.MessageTemplate.Text
        .Should().Be("My log message!");
}
```

You can also get a stream of log events as an observable, which can be useful for testing long running or asynchronous tasks.

```csharp
using (TestCorrelator.CreateContext())
{
    TestCorrelator.GetLogEventStreamFromCurrentContext()
        .Subscribe(logEvent => logEvent.MessageTemplate.Text.Should().Be("My log message!"));

    Log.Information("My log message!");
}
```

__New in v4:__ If you have more than one logger and want to filter to only log events emitted to a specific sink you can pass in an ID that you then use to filter the log events.

```csharp
TestCorrelatorSinkId firstTestCorrelatorSinkId = new();
ILogger firstLogger = new LoggerConfiguration().WriteTo.TestCorrelator(firstTestCorrelatorSinkId).CreateLogger();

TestCorrelatorSinkId secondTestCorrelatorSinkId = new();
ILogger secondLogger = new LoggerConfiguration().WriteTo.TestCorrelator(secondTestCorrelatorSinkId).CreateLogger();

using (TestCorrelator.CreateContext())
{
    firstLogger.Information("My first log message!");
    secondLogger.Information("My second log message!");

    TestCorrelator.GetLogEventsForSinksFromCurrentContext(firstTestCorrelatorSinkId)
        .Should().ContainSingle()
        .Which.MessageTemplate.Text
        .Should().Be("My first log message!");

    TestCorrelator.GetLogEventsForSinksFromCurrentContext(secondTestCorrelatorSinkId)
        .Should().ContainSingle()
        .Which.MessageTemplate.Text
        .Should().Be("My second log message!");
}
```

For more examples check out the [unit tests](https://github.com/MitchBodmer/serilog-sinks-testcorrelator/tree/master/test/Serilog.Sinks.TestCorrelator.Tests)!

## v4 Breaking Changes
Version 4 comes with a few breaking changes.
- All package dependencies have been updated.
- Dropped support for .Net Framework 4.6.1 and below. This brings the package in line with Serilog's supported frameworks.
- A new `TestCorrelatorContextId` class is not being used instead of `Guids`. The corresponding `...ContextGuid()` methods have been renamed to `...ContextId()`.
- `LogEvent`s emitted outside of a context are no longer captured.
- `LoggerConfiguration` extension methods with implicitly ignored minimum `LogEventLevel`` parameters were removed.