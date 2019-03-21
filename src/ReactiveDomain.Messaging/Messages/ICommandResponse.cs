using System;

namespace ReactiveDomain.Messaging
{
    public interface ICommandResponse
    {
        Guid CommandId { get; }
        Type CommandType { get; }
        ICommand SourceCommand { get; }
    }
}