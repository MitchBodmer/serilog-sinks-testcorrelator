using FluentAssertions;
using Xunit;

namespace Serilog.Utilities.ConcurrentCorrelator.Tests
{
    public class ModuleInitializerTests
    {
        [Fact]
        public void When_the_module_is_loaded_the_global_logger_is_not_a_SilentLogger()
        {
            //Force the module to load.
            using (new CorrelationLogContext()) 
            {
                Log.Logger.GetType().FullName.Should().NotBe("Serilog.Core.Pipeline.SilentLogger");
            }
        }
    }
}
