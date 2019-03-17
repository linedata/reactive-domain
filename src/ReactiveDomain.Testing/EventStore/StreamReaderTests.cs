using ReactiveDomain.Foundation;
using ReactiveDomain.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace ReactiveDomain.Testing.EventStore
{
    public class StreamReaderTests : IClassFixture<StreamStoreConnectionFixture>, Messaging.Bus.IHandle<Event>
    {
        private readonly List<IStreamStoreConnection> _stores = new List<IStreamStoreConnection>();
        private readonly IEventSerializer _serializer = new JsonMessageSerializer();
        private readonly string _streamName;
        private readonly IStreamNameBuilder _streamNameBuilder;
        private long _count;
        private readonly int NUM_OF_EVENTS = 10;
        private ITestOutputHelper _toh;

        public StreamReaderTests(ITestOutputHelper toh, StreamStoreConnectionFixture fixture)
        {
            _toh = toh;
            _streamNameBuilder = new PrefixedCamelCaseStreamNameBuilder("UnitTest");
            var mockStreamStore = new MockStreamStoreConnection("Test-" + Guid.NewGuid());
            mockStreamStore.Connect();

            _stores.Add(mockStreamStore);
            //_stores.Add(fixture.Connection);

            _streamName = _streamNameBuilder.GenerateForAggregate(typeof(TestAggregate), Guid.NewGuid());


            foreach (var store in _stores)
            {
                AppendEvents(NUM_OF_EVENTS, store, _streamName);

            }
        }

        private void AppendEvents(int numEventsToBeSent, IStreamStoreConnection conn, string streamName)
        {
            _toh.WriteLine($"Appending {numEventsToBeSent} events to stream \"{streamName}\" with connection {conn.ConnectionName}");

            for (int evtNumber = 0; evtNumber < numEventsToBeSent; evtNumber++)
            {
                var evt = new ReadTestEvent(evtNumber);
                conn.AppendToStream(streamName, ExpectedVersion.Any, null, _serializer.Serialize(evt));
            }
        }

        private void AppendEventArray(int numEventsToBeSent, IStreamStoreConnection conn, string streamName)
        {
            _toh.WriteLine($"Appending {numEventsToBeSent} events to stream \"{streamName}\" with connection {conn.ConnectionName}");

            var events = new Event[numEventsToBeSent];
            for (int evtNumber = 0; evtNumber < numEventsToBeSent; evtNumber++)
            {
                events[evtNumber] = new ReadTestEvent(evtNumber);
            }

            conn.AppendToStream(streamName, ExpectedVersion.Any, null, events.Select(x => _serializer.Serialize(x)).ToArray());
        }

        [Fact]
        public void can_read_stream_forward()
        {
            foreach (var conn in _stores)
            {
                var reader = new StreamReader("TestReader", conn, _streamNameBuilder, _serializer);

                reader.EventStream.Subscribe<Event>(this);

                reader.Read(_streamName);

                Assert.Equal(NUM_OF_EVENTS, _count);
            }
        }

        [Fact]
        public void can_read_stream_forward_from_position()
        {
            foreach (var conn in _stores)
            {
                var reader = new StreamReader("TestReader", conn, _streamNameBuilder, _serializer);

                var position = NUM_OF_EVENTS / 2;

                reader.EventStream.Subscribe<Event>(this);

                reader.Read(_streamName, position);

                Assert.Equal(NUM_OF_EVENTS - position, _count);
            }
        }

        [Fact]
        public void can_read_zero_events_forward_from_position()
        {
            foreach (var conn in _stores)
            {
                var reader = new StreamReader("TestReader", conn, _streamNameBuilder, _serializer);

                reader.EventStream.Subscribe<Event>(this);

                reader.Read(_streamName, NUM_OF_EVENTS);

                Assert.Equal(0, _count);
            }
        }



        [Fact]
        public void cannot_read_non_existing_stream()
        {
            foreach (var conn in _stores)
            {
                var reader = new StreamReader("TestReader", conn, _streamNameBuilder, _serializer);

                reader.EventStream.Subscribe<Event>(this);

                Assert.Throws<ArgumentException>(() =>
                {
                    reader.Read("missing_stream");
                });
            }
        }

        [Fact]
        public void can_read_forward_updating_stream()
        {
            foreach (var conn in _stores)
            {
                var reader = new StreamReader("TestReader", conn, _streamNameBuilder, _serializer);

                reader.EventStream.Subscribe<Event>(this);

                Parallel.Invoke(
                    () =>
                    {
                        for (int chunkNum = 0; chunkNum < 10; chunkNum++) 
                            AppendEventArray(NUM_OF_EVENTS, conn, _streamName);
                    },
                    () => reader.Read(_streamName)
                    );

                _toh.WriteLine($"Read events: {_count}");
                Assert.Equal(0, _count % NUM_OF_EVENTS);
                
            }
        }

        [Fact]
        public void can_read_stream_backward()
        {
            foreach (var conn in _stores)
            {
                var reader = new StreamReader("TestReader", conn, _streamNameBuilder, _serializer);

                reader.EventStream.Subscribe<Event>(this);

                reader.Read(_streamName, readBackwards: true);

                Assert.Equal(NUM_OF_EVENTS, _count);
            }
        }

        [Fact]
        public void can_read_stream_backward_from_position()
        {
            foreach (var conn in _stores)
            {
                var reader = new StreamReader("TestReader", conn, _streamNameBuilder, _serializer);

                var position = NUM_OF_EVENTS / 2;

                reader.EventStream.Subscribe<Event>(this);

                reader.Read(_streamName, position, readBackwards: true);

                Assert.Equal(position + 1, _count); // events from positions: N, N-1...0 => N+1 events
            }
        }

        [Fact(Skip = "wip, not implemented yet")]
        public void can_read_stream_backward_multiple_slices()
        {
            foreach (var conn in _stores)
            {
                var reader = new StreamReader("TestReader", conn, _streamNameBuilder, _serializer);

                reader.EventStream.Subscribe<Event>(this);

                reader.Read(_streamName, readBackwards: true);

                Assert.Equal(NUM_OF_EVENTS, _count);
            }
        }


        public void Handle(Event message)
        {
            //Thread.Sleep(new Random().Next(300)); // random event handling time
            Interlocked.Increment(ref _count);
        }

        public class ReadTestEvent : Event
        {
            public readonly int MessageNumber;
            public ReadTestEvent(
                int messageNumber
            ) : base(NewRoot())
            {
                MessageNumber = messageNumber;
            }
        }
    }
}