# Serilog Concurrent Correlator

The Serilog Concurrent Correlator is a small library for unit testing Serilog log events in concurrent testing frameworks.

# The Problem

Logging libraries like [Serilog](https://github.com/serilog/serilog) often provide a logger as a static resource to avoid the clutter of having to pass a logger to every class that needs to use one. While convenient in product code, this can often cause a headache when unit testing log output due to the difficulty of determining which test produced which LogEvent.

# Our Solution

This library provides four tools to help you correlate your LogEvents to the test that produced them:
* A module initializer, which configures Serilog's global Logger when the module is loaded.
* A static global SerilogLogEvents.Bag, which contains all the LogEvents your tests produce.
* A disposable CorrelationLogContext, which you can create in a using block to enrich all LogEvents within the block's logical call context with a CorrelationGuid property.
* An extension method for filtering an IEnumerable\<LogEvent\> to those with a specific CorrelationGuid.

# Examples

## Basic Usage
```csharp
using (var correlationLogContext = new CorrelationLogContext())
{
    Log.Logger.Information("Message template.");

    SerilogLogEvents.Bag.WithCorrelationLogContextGuid(correlationLogContext.Guid)
        .Should()
        .HaveCount(1);
}
```

## Concurrency
```csharp
using (var context = new CorrelationLogContext())
{
    var logTask = Task.Run(() =>
    {
        Log.Logger.Information("Message template.");
    });

    Task.WaitAll(logTask);

    SerilogLogEvents.Bag.WithCorrelationLogContextGuid(context.Guid)
        .Should()
        .HaveCount(1);
}
```

For more examples check out the unit tests!

# Contributing

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/). For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.
