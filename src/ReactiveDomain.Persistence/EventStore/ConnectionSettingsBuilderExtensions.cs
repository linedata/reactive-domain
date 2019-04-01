using System;
using EventStore.ClientAPI;

namespace ReactiveDomain.EventStore
{
    public static class ConnectionSettingsBuilderExtensions {

        public static T IfVerboseLogging<T>(this T t, Func<bool> cond, Func<T, T> builder) where T : ConnectionSettingsBuilder {
            return cond() ? builder(t) : t;
        }
    }
}
