using Serilog;
using Serilog.Utilities.ConcurrentCorrelator;

// Used by the ModuleInit. All code inside the Initialize method is ran as soon as the assembly is loaded.
public static class ModuleInitializer
{
    public static void Initialize()
    {
        Log.Logger =
            new LoggerConfiguration().WriteTo.ProducerConsumerCollection(
                    SerilogLogEvents.Bag)
                .Enrich.FromLogContext()
                .CreateLogger();
    }
}