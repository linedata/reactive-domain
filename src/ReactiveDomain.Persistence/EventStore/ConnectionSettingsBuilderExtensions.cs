using System;
using EventStore.ClientAPI;

namespace ReactiveDomain.EventStore
{
    internal static class ConnectionSettingsBuilderExtensions {

        internal static T If<T>(this T t, Func<bool> cond, Func<T, T> builder) where T : ConnectionSettingsBuilder {
            return cond() ? builder(t) : t;
        }
    }
}
