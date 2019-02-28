using System;

namespace ReactiveDomain.Messaging.Messages {
    public interface IChainedMessage {
        Guid MsgId { get; }
        Guid SourceId { get; }
        Guid CorrelationId { get; }
        Guid PrincipalId { get; }
    }
}