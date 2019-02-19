using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using ReactiveDomain.Messaging;
using System;
using System.Reflection;
using Newtonsoft.Json.Utilities;

// ReSharper disable once CheckNamespace
namespace ReactiveDomain.Foundation
{
    public class JsonMessageSerializer : IEventSerializer
    {
        private static readonly JsonSerializerSettings SerializerSettings;
        public const string EventClrQualifiedTypeHeader = "EventClrQualifiedTypeName";
        public const string EventClrTypeHeader = "EventClrTypeName";

        static JsonMessageSerializer()
        {
            SerializerSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Converters = new JsonConverter[]
                {
                    new SourceId.SourceIdGuidConverter(),
                    new CorrelationId.CorrelationIdGuidConverter()
                },
                ContractResolver = new FieldBasedResolver()

            };
        }
        public EventData Serialize(object @event, IDictionary<string, object> headers = null)
        {

            if (headers == null)
            {
                headers = new Dictionary<string, object>();
            }

            try
            {
                headers.Add(EventClrTypeHeader, @event.GetType().Name);
                headers.Add(EventClrQualifiedTypeHeader, @event.GetType().AssemblyQualifiedName);
            }
            catch (Exception e)
            {
                var msg = e.Message;

            }
            var metadata = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(headers, SerializerSettings));
            var dString = JsonConvert.SerializeObject(@event, SerializerSettings);
            var data = Encoding.UTF8.GetBytes(dString);
            var typeName = @event.GetType().Name;

            return new EventData(Guid.NewGuid(), typeName, true, data, metadata);
        }

        public object Deserialize(IEventData @event)
        {

            var eventClrTypeName = JObject.Parse(
                                Encoding.UTF8.GetString(@event.Metadata)).Property(EventClrQualifiedTypeHeader).Value; // todo: fallback to using type name optionally
            return JsonConvert.DeserializeObject(
                                Encoding.UTF8.GetString(@event.Data),
                                Type.GetType((string)eventClrTypeName),
                                SerializerSettings);
        }


    }





    /// <summary>
    /// Creates a custom object.
    /// </summary>
    /// <typeparam name="T">The object type to convert.</typeparam>
    public abstract class CustomCreationConverter<T> : JsonConverter
    {
        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The <see cref="JsonWriter"/> to write to.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The calling serializer.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotSupportedException("CustomCreationConverter should only be used while deserializing.");
        }

        /// <summary>
        /// Reads the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="JsonReader"/> to read from.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="existingValue">The existing value of object being read.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <returns>The object value.</returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }
            FormatterServices.GetSafeUninitializedObject
            T value = Create(objectType);
            if (value == null)
            {
                throw new JsonSerializationException("No object created.");
            }

            serializer.Populate(reader, value);
            return value;
        }

        /// <summary>
        /// Creates an object which will then be populated by the serializer.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns>The created object.</returns>
        public abstract T Create(Type objectType);

        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns>
        /// 	<c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanConvert(Type objectType)
        {
            return typeof(T).IsAssignableFrom(objectType);
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="JsonConverter"/> can write JSON.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this <see cref="JsonConverter"/> can write JSON; otherwise, <c>false</c>.
        /// </value>
        public override bool CanWrite => false;
    }
}

