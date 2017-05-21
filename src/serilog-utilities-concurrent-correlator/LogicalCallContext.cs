using System;
using System.Collections.Immutable;
#if ASYNCLOCAL
using System.Threading;
#elif REMOTING
using System.Runtime.Remoting.Messaging;
#endif

namespace Serilog.Utilities.ConcurrentCorrelator
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
            GuidAsyncLocal.Value = GetOrCreateGuids().Add(guid);
#elif REMOTING
            CallContext.LogicalSetData(DataSlotName, GetOrCreateGuids().Add(guid));
#endif
        }

        public static void Remove(Guid guid)
        {
#if ASYNCLOCAL
            GuidAsyncLocal.Value = GuidAsyncLocal.Value.Remove(guid);
#elif REMOTING
            CallContext.LogicalSetData(DataSlotName, GetOrCreateGuids().Remove(guid));
#endif
        }

        public static bool Contains(Guid guid)
        {
#if ASYNCLOCAL
            return GuidAsyncLocal.Value.Contains(guid);
#elif REMOTING
            if (CallContext.LogicalGetData(DataSlotName) is ImmutableList<Guid> guids)
            {
                return guids.Contains(guid);
            }
            return false;
#endif
        }

        static ImmutableList<Guid> GetOrCreateGuids()
        {
#if ASYNCLOCAL
            return GuidAsyncLocal.Value ?? ImmutableList<Guid>.Empty;
#elif REMOTING
            return CallContext.LogicalGetData(DataSlotName) as ImmutableList<Guid> ?? ImmutableList<Guid>.Empty;
#endif
        }
    }
}