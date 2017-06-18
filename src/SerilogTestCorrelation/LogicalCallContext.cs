using System;
using System.Collections.Immutable;
#if ASYNCLOCAL
using System.Threading;
#elif REMOTING
using System.Runtime.Remoting.Messaging;
#endif

namespace SerilogTestCorrelation
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
            CallContext.LogicalSetData(DataSlotName, GetOrCreateGuidList().Add(guid));
#endif
        }

        public static void Remove(Guid guid)
        {
#if ASYNCLOCAL
            GuidAsyncLocal.Value = GetOrCreateGuidList().Remove(guid);
#elif REMOTING
            CallContext.LogicalSetData(DataSlotName, GetOrCreateGuidList().Remove(guid));
#endif
        }

        public static bool Contains(Guid guid)
        {
            return GetOrCreateGuidList().Contains(guid);
        }

        static ImmutableList<Guid> GetOrCreateGuidList()
        {
            return
#if ASYNCLOCAL
                GuidAsyncLocal.Value ??
#elif REMOTING
                CallContext.LogicalGetData(DataSlotName) as ImmutableList<Guid> ??
#endif
                ImmutableList<Guid>.Empty;
        }
    }
}