using System;

namespace ReactiveDomain.Messaging {
    public interface IChainedMessage {
        Guid MsgId { get; }
        SourceId SourceId { get; }
        CorrelationId CorrelationId { get; }
        Guid PrincipalId { get; }
    }
}