namespace ReactiveDomain.Messaging
{
    public interface ICorrelatedMessage
    {
        CorrelationId CorrelationId { get; }
        SourceId SourceId { get; }
    }
}