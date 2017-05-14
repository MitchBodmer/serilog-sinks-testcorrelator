using System;
using System.Runtime.Remoting.Messaging;
using Serilog.Context;

namespace Serilog.Utilities.ConcurrentCorrelator
{
    class TestLogContext : ITestLogContext
    {
        public TestLogContext()
        {
            Guid = Guid.NewGuid();
            EnterLogicalCallContextIntoTestLogContext();
        }

        public Guid Guid { get; }

        public void Dispose()
        {
            RemoveLogicalCallContextFromTestLogContext(); 
        }

        void EnterLogicalCallContextIntoTestLogContext()
        {
            CallContext.LogicalSetData(Guid.ToString(), new object());
        }

        void RemoveLogicalCallContextFromTestLogContext()
        {
            CallContext.FreeNamedDataSlot(Guid.ToString());
        }
    }
}