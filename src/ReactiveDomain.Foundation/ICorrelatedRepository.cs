using System;
using ReactiveDomain.Messaging;

namespace ReactiveDomain.Foundation {
    public interface ICorrelatedRepository
    {
        bool TryGetById<TAggregate>(Guid id, out TAggregate aggregate, ICorrelatedMessage source) where TAggregate : CorrelatedEDSM, IEventSource;
        bool TryGetById<TAggregate>(Guid id, int version, out TAggregate aggregate, ICorrelatedMessage source) where TAggregate : CorrelatedEDSM, IEventSource;
        TAggregate GetById<TAggregate>(Guid id, ICorrelatedMessage source) where TAggregate : CorrelatedEDSM, IEventSource;
        TAggregate GetById<TAggregate>(Guid id, int version, ICorrelatedMessage source) where TAggregate : CorrelatedEDSM, IEventSource;
        void Save(IEventSource aggregate);
    }
}