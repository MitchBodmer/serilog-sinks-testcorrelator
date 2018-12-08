# Serilog.Sinks.TestCorrelator [![AppVeyor Badge](https://ci.appveyor.com/api/projects/status/wf2emjam7xsviebw/branch/master?svg=true)](https://ci.appveyor.com/project/SerilogUtilitiesConcurrentCorrelatorSA/serilog-utilities-concurrent-correlator/branch/master) [![NuGet Badge](https://buildstats.info/nuget/Serilog.Sinks.TestCorrelator)](https://www.nuget.org/packages/Serilog.Sinks.TestCorrelator/)

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

For more examples check out the [unit tests](https://github.com/Microsoft/serilog-sinks-testcorrelator/tree/master/test/Serilog.Sinks.TestCorrelator.Tests)!

## Contributing

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/). For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.