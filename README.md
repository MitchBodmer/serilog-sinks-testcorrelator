# Serilog Concurrent Correlator

[![AppVeyor Badge](https://ci.appveyor.com/api/projects/status/wf2emjam7xsviebw/branch/master?svg=true)](https://ci.appveyor.com/project/SerilogUtilitiesConcurrentCorrelatorSA/serilog-utilities-concurrent-correlator/branch/master)
[![NuGet Badge](https://buildstats.info/nuget/Serilog.Utilities.ConcurrentCorrelator)](https://www.nuget.org/packages/Serilog.Utilities.ConcurrentCorrelator/)

The Serilog Concurrent Correlator is a small library for unit testing Serilog logging in concurrent testing frameworks.

## The Problem

Logging libraries like [Serilog](https://github.com/serilog/serilog) often provide a logger as a static resource to avoid the clutter of having to pass a logger to every class that needs one. While convenient in product code, this pattern can often cause a headache when unit testing log output due to the difficulty of determining which test produced which LogEvent.

## Our Solution

This library provides tools to help you correlate your ```LogEvents``` to the test that produced them:
* ```ConfigureGlobalLoggerForTesting``` allows you to configure the global logger to output all ```LogEvents``` to a single thread safe collection.
* ```EstablishTestLogContext``` enriches each LogEvent written within its logical call context with a context identifier.
* ```GetLogEventsWithContextIdentifier``` allows you filter down to the LogEvents enriched with a specific context identifier.

## Examples

### Initialization
Put this line wherever pre-test setup happens in your test framework.

```csharp
TestSerilogLogEvents.ConfigureGlobalLoggerForTesting();
```

### Basic Usage

```csharp
public void TestMethod()
{
    using (var context = TestSerilogLogEvents.EstablishTestLogContext())
    {
        Log.Information("My information log!");

        TestSerilogLogEvents.GetLogEventsWithContextIdentifier(context.Identifier).Should().ContainSingle();
    }
}
```

### Concurrency

```csharp
public void ConcurrencyTestMethod()
{
    using (var context = TestSerilogLogEvents.EstablishTestLogContext())
    {
        var logTask = Task.Run(() => { Log.Information("My information log!"); });

        Task.WaitAll(logTask);

        TestSerilogLogEvents.GetLogEventsWithContextIdentifier(context.Identifier).Should().ContainSingle();
    }
}
```

For more examples check out the [unit tests](https://github.com/Microsoft/serilog-utilities-concurrent-correlator/tree/master/serilog-utilities-concurrent-correlator-tests)!

## Contributing

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/). For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.
