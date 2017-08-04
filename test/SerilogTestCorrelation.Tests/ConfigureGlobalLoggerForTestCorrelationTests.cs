using Serilog;

namespace SerilogTestCorrelation.Tests
{
    public partial class SerilogTestCorrelatorTests
    {
        public SerilogTestCorrelatorTests()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Sink(new TestCorrelatorSink())
                .Enrich.FromLogContext()
                .CreateLogger();
        }
    }
}