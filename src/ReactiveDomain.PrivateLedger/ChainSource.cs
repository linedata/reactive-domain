using System;
using Newtonsoft.Json;
using ReactiveDomain.Messaging;

namespace ReactiveDomain.PrivateLedger
{
    public static class ChainSourcehelpers
    {
        public static ChainSource GetMemento(this IChainedMessage source)
        {
            return new ChainSource(source);
        }
    }
    public class ChainSource : IChainedMessage
    {
        public Guid MsgId { get; }
        public SourceId SourceId { get; }
        public CorrelationId CorrelationId { get; }
        public Guid PrincipalId { get; }

        public ChainSource(IChainedMessage source)
            : this(source.MsgId, source.SourceId, source.CorrelationId, source.PrincipalId)
        {
        }
        [JsonConstructor]
        public ChainSource(
            Guid msgId,
            SourceId sourceId,
            CorrelationId correlationId,
            Guid principalId)
        {
            MsgId = msgId;
            SourceId = sourceId;
            CorrelationId = correlationId;
            PrincipalId = principalId;
        }

    }
}