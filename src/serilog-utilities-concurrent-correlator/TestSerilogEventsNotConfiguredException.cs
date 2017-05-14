using System;
using System.Runtime.Serialization;

namespace Serilog.Utilities.ConcurrentCorrelator
{
    [Serializable]
    public class TestSerilogEventsNotConfiguredException : Exception
    {
        public TestSerilogEventsNotConfiguredException() :
            base(
                $"Serilog's global logger has not been configured for testing. This can either be because you did not call {nameof(TestSerilogLogEvents.ConfigureGlobalLoggerForTesting)}, or because other code has overwritten {nameof(Serilog.Log.Logger)} since you did.")
        { }

        protected TestSerilogEventsNotConfiguredException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}