using System;
using System.Threading;
using ReactiveDomain.Messaging;
using ReactiveDomain.Messaging.Bus;

// ReSharper disable once CheckNamespace
namespace ReactiveDomain.Foundation
{
    /// <summary>
    /// StreamReader
    /// This class reads streams on a Subscribable bus and is primarily used in the building of read models. 
    /// The Raw events returned from the Stream will be unwrapped using the provided serializer and
    /// consumers can subscribe to event notifications by subscribing to the exposed EventStream.
    ///</summary>

    public class StreamReader : IStreamReader
    {
        protected readonly string ReaderName;
        protected readonly InMemoryBus Bus;
        private readonly IStreamNameBuilder _streamNameBuilder;
        protected readonly IEventSerializer Serializer;
        public ISubscriber EventStream => Bus;
        private readonly IStreamStoreConnection _streamStoreConnection;
        protected long StreamPosition;
        public long Position => StreamPosition;
        public string StreamName { get; private set; }

        /// <summary>
        /// Create a stream Reader
        /// </summary>
        /// <param name="name">Name of the reader</param>
        /// <param name="streamStoreConnection">The stream store to subscribe to</param>
        /// <param name="streamNameBuilder">The source for correct stream names based on aggregates and events</param>
        /// <param name="serializer">the serializer to apply to the evenets in the stream</param>
        /// <param name="busName">The name to use for the internal bus (helpful in debugging)</param>
        public StreamReader(
                string name,
                IStreamStoreConnection streamStoreConnection,
                IStreamNameBuilder streamNameBuilder,
                IEventSerializer serializer,
                string busName = null)
        {
            ReaderName = name ?? nameof(StreamReader);
            _streamStoreConnection = streamStoreConnection ?? throw new ArgumentNullException(nameof(streamStoreConnection));
            _streamNameBuilder = streamNameBuilder ?? throw new ArgumentNullException(nameof(streamNameBuilder));
            Serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            Bus = new InMemoryBus(busName ?? $"{ReaderName} {nameof(EventStream)}");
        }
        /// <summary>
        /// By Event Type Projection Reader
        /// i.e. $et-[MessageType]
        /// </summary>
        /// <param name="tMessage"></param>
        /// <param name="checkpoint"></param>
        /// <param name="readBackwards"></param>
        public void Read(
            Type tMessage,
            long? checkpoint = null,
            bool readBackwards = false)
        {
            if (!tMessage.IsSubclassOf(typeof(Event)))
            {
                throw new ArgumentException("type must derive from ReactiveDomain.Messaging.Event", nameof(tMessage));
            }
            Read(
                _streamNameBuilder.GenerateForEventType(tMessage.Name),
               checkpoint,
               readBackwards);
        }
        /// <summary>
        /// By Category Projection Stream Reader
        /// i.e. $ce-[AggregateType]
        /// </summary>
        /// <typeparam name="TAggregate">The Aggregate type used to generate the stream name</typeparam>
        /// <param name="checkpoint"></param>
        /// <param name="readBackwards"></param>
        public void Read<TAggregate>(
                        long? checkpoint = null,
            bool readBackwards = false) where TAggregate : class, IEventSource
        {
            Read(
               _streamNameBuilder.GenerateForCategory(typeof(TAggregate)),
               checkpoint,
               readBackwards);
        }

        /// <summary>
        /// Aggregate-[id] Stream Reader
        /// i.e. [AggregateType]-[id]
        /// </summary>
        /// <typeparam name="TAggregate">The Aggregate type used to generate the stream name</typeparam>
        /// <param name="id"></param>
        /// <param name="checkpoint"></param>
        /// <param name="readBackwards"></param>
        public void Read<TAggregate>(
                        Guid id,
                        long? checkpoint = null,
                        bool readBackwards = false) where TAggregate : class, IEventSource
        {
            Read(
                _streamNameBuilder.GenerateForAggregate(typeof(TAggregate), id),
                checkpoint,
                readBackwards);
        }

        /// <summary>
        /// Named Stream Reader
        /// i.e. [StreamName]
        /// </summary>
        /// <param name="streamName"></param>
        /// <param name="checkpoint"></param>
        /// <param name="readBackwards"></param>
        public virtual void Read(
                            string streamName,
                            long? checkpoint = null,
                            bool readBackwards = false)
        {
            if (readBackwards) { throw new NotImplementedException("Cannot read backwards"); }
            if (!ValidateStreamName(streamName))
                throw new ArgumentException("Stream not found.", streamName);

            StreamName = streamName;
            long sliceStart = 0;
            StreamEventsSlice currentSlice;
            do
            {
                currentSlice = _streamStoreConnection.ReadStreamForward(streamName, sliceStart, 500);

                sliceStart = currentSlice.NextEventNumber;
                Array.ForEach(currentSlice.Events, EventRead);

            } while (!currentSlice.IsEndOfStream);
        }
        public bool ValidateStreamName(string streamName)
        {
            var currentSlice = _streamStoreConnection.ReadStreamForward(streamName, 0, 1);
            return (currentSlice is StreamNotFoundSlice) || (currentSlice is StreamDeletedSlice);
        }
        protected virtual void EventRead(RecordedEvent recordedEvent)
        {
            Interlocked.Exchange(ref StreamPosition, recordedEvent.EventNumber);
            if (Serializer.Deserialize(recordedEvent) is Message @event)
            {
                Bus.Publish(@event);
            }
        }
        #region Implementation of IDisposable
        private bool _disposed;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;
            Bus?.Dispose();
            _disposed = true;
        }
        #endregion
    }
}