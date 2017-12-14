using System;
using System.Collections.Immutable;
#if ASYNCLOCAL
using System.Threading;
#elif REMOTING
using System.Runtime.Remoting;
using System.Runtime.Remoting.Messaging;
#endif

namespace Serilog.Sinks.TestCorrelator
{
    static class LogicalCallContext
    {
#if ASYNCLOCAL
        static readonly AsyncLocal<ImmutableList<Guid>> GuidAsyncLocal = new AsyncLocal<ImmutableList<Guid>>();
#elif REMOTING
        static readonly string DataSlotName = Guid.NewGuid().ToString();
#endif

        public static void Add(Guid guid)
        {
#if ASYNCLOCAL
            GuidAsyncLocal.Value = GetOrCreateGuidList().Add(guid);
#elif REMOTING
            CallContext.LogicalSetData(DataSlotName, new ObjectHandle(GetOrCreateGuidList().Add(guid)));
#endif
        }

        public static void Remove(Guid guid)
        {
#if ASYNCLOCAL
            GuidAsyncLocal.Value = GetOrCreateGuidList().Remove(guid);
#elif REMOTING
            CallContext.LogicalSetData(DataSlotName, new ObjectHandle(GetOrCreateGuidList().Remove(guid)));
#endif
        }

        public static bool Contains(Guid guid)
        {
            return GetOrCreateGuidList().Contains(guid);
        }

        static ImmutableList<Guid> GetOrCreateGuidList()
        {
#if ASYNCLOCAL
            return GuidAsyncLocal.Value ?? ImmutableList<Guid>.Empty;
#elif REMOTING
            if (CallContext.LogicalGetData(DataSlotName) is ObjectHandle objectHandle)
            {
                return objectHandle.Unwrap() as ImmutableList<Guid>;
            }

            return ImmutableList<Guid>.Empty;
#endif
        }
    }
}