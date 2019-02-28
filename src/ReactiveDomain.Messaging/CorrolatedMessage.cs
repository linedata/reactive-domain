using System;

namespace ReactiveDomain.Messaging
{
    public class CorrelatedMessage: Message
    {
        public Guid CorrelationId { get; }
        public Guid SourceId { get; }

        public CorrelatedMessage(CorrelationId correlationId, SourceId sourceId) {
            CorrelationId = correlationId;
            SourceId = sourceId;
        }
        private CorrelatedMessage(CorrelationId correlationId) {
            CorrelationId = correlationId;
            SourceId = Messaging.SourceId.NullSourceId();
        }

        public static CorrelatedMessage NewRoot() {
            return new CorrelatedMessage(Messaging.CorrelationId.NewId());
        }
    }

    
}
