﻿using ReactiveDomain.Messaging.Bus;
using System;


namespace ReactiveDomain.Foundation
{
    public interface IStreamReader : IDisposable
    {
        /// <summary>
        /// The Eventstream the Events are read onto
        /// </summary>
        ISubscriber EventStream { get; }
        
        /// <summary>
        /// The ending position of the stream after the read is complete
        /// </summary>
        long? Position { get; }
        /// <summary>
        /// The name of the stream being read
        /// </summary>
        string StreamName { get; }

        /// <summary>
        /// Reads the events on a named stream
        /// </summary>
        /// <param name="stream">the exact stream name</param>
        /// <param name="checkpoint">start point to listen from</param>
        /// <param name="count">The count of items to read</param>
        /// <param name="readBackwards">read the stream backwards</param>
        void Read(string stream, long? checkpoint = null, long? count = null, bool readBackwards = false);

        /// <summary>
        /// Reads the events on an aggregate root stream
        /// </summary>
        /// <typeparam name="TAggregate">The type of aggregate</typeparam>
        /// <param name="id">the aggregate id</param>
        /// <param name="checkpoint">start point to listen from</param>
        /// <param name="count">The count of items to read</param>
        /// <param name="readBackwards">read the stream backwards</param>
        void Read<TAggregate>(Guid id, long? checkpoint = null, long? count = null, bool readBackwards = false) where TAggregate : class, IEventSource;

        /// <summary>
        /// Reads the events on a Aggregate Category Stream
        /// </summary>
        /// <typeparam name="TAggregate">The type of aggregate</typeparam>
        /// <param name="checkpoint">start point to listen from</param>
        /// <param name="count">The count of items to read</param>
        /// <param name="readBackwards">read the stream backwards</param>
        void Read<TAggregate>(long? checkpoint = null, long? count = null, bool readBackwards = false) where TAggregate : class, IEventSource;


        /// <summary>
        /// Interrupts the reading process. Doesn't guarantee the moment when reading is stopped. For optimization purpose.
        /// </summary>
        void Cancel();
    }
}
