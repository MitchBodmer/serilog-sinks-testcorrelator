using System;
using System.Collections.Immutable;
using System.Threading;

namespace Serilog.Sinks.TestCorrelator
{
    static class LogicalCallContext
    {
        static readonly AsyncLocal<ImmutableList<Guid>> GuidAsyncLocal = new AsyncLocal<ImmutableList<Guid>>();

        public static void Add(Guid guid)
        {
            GuidAsyncLocal.Value = GetOrCreateGuidList().Add(guid);

        }

        public static void Remove(Guid guid)
        {
            GuidAsyncLocal.Value = GetOrCreateGuidList().Remove(guid);
        }

        public static bool Contains(Guid guid)
        {
            return GetOrCreateGuidList().Contains(guid);
        }

        static ImmutableList<Guid> GetOrCreateGuidList()
        {
            return GuidAsyncLocal.Value ?? ImmutableList<Guid>.Empty;
        }
    }
}