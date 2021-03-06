﻿using System;
using System.Collections.Generic;

namespace ReactiveDomain.Foundation.StreamStore {
    /// <summary>
    /// This implementation assumes that the cached aggregate may have changes since it was last seen
    ///
    /// Therefore the aggregate is cached on first retrieve and the store is checked for updates upon
    /// each subsequent retrieval
    ///
    /// Also retrieving an aggregate at anything other than latest is disabled
    ///
    /// Cache management (e.g. eviction) is the responsibility of the caller/external owner
    ///
    /// Save failures will clear the aggregate from the cache and rethrow
    /// </summary>
    public class CachingRepository : IDisposable {

        private readonly IRepository _baseRepository;
        private readonly Dictionary<Guid, IEventSource> _knownAggregates = new Dictionary<Guid, IEventSource>();
        public CachingRepository(IRepository baseRepository) {
            _baseRepository = baseRepository;
        }


        public bool TryGetById<TAggregate>(Guid id, out TAggregate aggregate) where TAggregate : class, IEventSource {
            try {
                aggregate = GetById<TAggregate>(id);
                return true;
            }
            catch (Exception) {
                aggregate = null;
                return false;
            }
        }
        public TAggregate GetById<TAggregate>(Guid id) where TAggregate : class, IEventSource {
            if (_knownAggregates.TryGetValue(id, out var aggregate)) {
                try {
                    _baseRepository.UpdateToCurrent(aggregate);
                }
                catch(InvalidOperationException ex) {
                    throw new Exception("Persisted version changed with recorded events in aggregate.", ex);
                }
                catch (AggregateVersionException ex) {
                    throw new Exception("Persisted version mismatch.", ex);
                }
                return aggregate as TAggregate;
            }

            aggregate = _baseRepository.GetById<TAggregate>(id);
            _knownAggregates.Add(id, aggregate);

            return (TAggregate)aggregate;
        }
        public void Save(IEventSource aggregate) {
            try {
                _baseRepository.Save(aggregate);
                if (!_knownAggregates.ContainsKey(aggregate.Id)) {
                    _knownAggregates.Add(aggregate.Id, aggregate);
                }
            }
            catch {
                _knownAggregates.Remove(aggregate.Id);
                throw;
            }
        }
        public bool ClearCache(Guid id) {
            return _knownAggregates.Remove(id);
        }
        public void ClearCache() {
            _knownAggregates.Clear();
        }

        protected virtual void Dispose(bool disposing) {
            if (disposing) {
                _knownAggregates.Clear();
            }
        }
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}