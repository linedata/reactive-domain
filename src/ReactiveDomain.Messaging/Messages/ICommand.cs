using System;

namespace ReactiveDomain.Messaging
{
    public interface ICommand : IMessage
    {
        bool IsCancelable { get; }
        bool IsCanceled { get; }

        void RegisterOnCancellation(Action action);
        CommandResponse Succeed();
        CommandResponse Fail(Exception ex = null);
        CommandResponse Canceled();
    }
}