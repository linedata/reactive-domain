using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using ReactiveDomain.Messaging;

// ReSharper disable once CheckNamespace
namespace ReactiveDomain.Foundation {
    public class JsonMessageSerializer : IEventSerializer {
        private static readonly JsonSerializerSettings SerializerSettings;
        public const string EventClrQualifiedTypeHeader = "EventClrQualifiedTypeName";
        public const string EventClrTypeHeader = "EventClrTypeName";

        static JsonMessageSerializer() {
            SerializerSettings = new JsonSerializerSettings {
                TypeNameHandling = TypeNameHandling.Auto,
                Converters = new JsonConverter[]
                {
                    new SourceId.SourceIdGuidConverter(),
                    new CorrelationId.CorrelationIdGuidConverter()
                },
                ContractResolver = new FieldBasedResolver()
                
            };
        }
        public EventData Serialize(object @event, IDictionary<string, object> headers = null) {

            if (headers == null) {
                headers = new Dictionary<string, object>();
            }

            try {
                headers.Add(EventClrTypeHeader, @event.GetType().Name);
                headers.Add(EventClrQualifiedTypeHeader, @event.GetType().AssemblyQualifiedName);
            }
            catch (Exception e) {
                var msg = e.Message;
                
            }
            var metadata = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(headers, SerializerSettings));
            var dString = JsonConvert.SerializeObject(@event, SerializerSettings);
            var data = Encoding.UTF8.GetBytes(dString);
            var typeName = @event.GetType().Name;

            return new EventData(Guid.NewGuid(), typeName, true, data, metadata);
        }

        public object Deserialize(IEventData @event) {

            var eventClrTypeName = JObject.Parse(
                                Encoding.UTF8.GetString(@event.Metadata)).Property(EventClrQualifiedTypeHeader).Value; // todo: fallback to using type name optionally
            return JsonConvert.DeserializeObject(
                                Encoding.UTF8.GetString(@event.Data),
                                Type.GetType((string)eventClrTypeName),
                                SerializerSettings);
        }

        private class FieldBasedResolver : DefaultContractResolver
        {
            protected override IList<JsonProperty> CreateConstructorParameters(ConstructorInfo constructor, JsonPropertyCollection memberProperties)
            {
                return base.CreateConstructorParameters(constructor, memberProperties);
            }
            protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization) {
                if (type.IsSubclassOf(typeof(Message)) || type == typeof(Message)) {
                    return base.CreateProperties(type, MemberSerialization.Fields);
                }
                return base.CreateProperties(type, memberSerialization);
            }
            protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
            {
                var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                .Select(p => base.CreateProperty(p, memberSerialization))
                            .Union(type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                                       .Select(f => base.CreateProperty(f, memberSerialization)))
                            .ToList();
                props.ForEach(p => { p.Writable = true; p.Readable = true; });
                return props;
            }
        }
    }
}
