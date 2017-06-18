# Serilog Test Correlation

[![AppVeyor Badge](https://ci.appveyor.com/api/projects/status/wf2emjam7xsviebw/branch/master?svg=true)](https://ci.appveyor.com/project/SerilogUtilitiesConcurrentCorrelatorSA/serilog-utilities-concurrent-correlator/branch/master)
[![NuGet Badge](https://buildstats.info/nuget/Serilog.Utilities.ConcurrentCorrelator)](https://www.nuget.org/packages/Serilog.Utilities.ConcurrentCorrelator/)

Serilog Test Correlation is a library for unit testing Serilog logging.

## The Problem

Logging libraries like [Serilog](https://github.com/serilog/serilog) often provide a logger as a static resource to avoid the clutter of having to pass a logger to every class that needs one. While convenient in product code, this pattern can often cause a headache when unit testing log output due to the difficulty of determining which test produced which LogEvent.

## Our Solution

This library provides a ```SerilogTestCorrelator``` to help you correlate your ```LogEvent```s to the test code that produced them.
* ```ConfigureGlobalLoggerForTestCorrelation``` configures Serilog's global logger to emit all ```LogEvent```s to a thread safe collection.
* ```CreateTestCorrelationContext``` creates an ```ITestCorrelationContext``` with a GUID identifier.
* ```GetLogEventsFromTestCorrelationContext``` returns the ```LogEvent```s emitted within the ```ITestCorrelationContext``` with the provided GUID identifier.

## Examples

### Initialization
Put this line wherever pre-test setup happens in your test framework.

```csharp
SerilogTestCorrelator.ConfigureGlobalLoggerForTestCorrelation();
```

### Usage

```csharp
public void TestMethod()
{
    using (var context = SerilogTestCorrelator.CreateTestCorrelationContext())
    {
        Log.Information("My log message.");

        SerilogTestCorrelator.GetLogEventsFromTestCorrelationContext(context.Guid).Should().ContainSingle();
    }
}
```

For more examples check out the [unit tests](https://github.com/Microsoft/serilog-test-correlation/tree/master/test/SerilogTestCorrelation.Tests)!

## Contributing

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/). For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.