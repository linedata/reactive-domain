using System;
using ReactiveDomain.Foundation;
using ReactiveDomain.Messaging.Bus;

namespace ReactiveDomain.Persistence.Samples
{
    public class TestReadModel :
        ReadModelBase,
        IHandle<AddTestData>
    {
        public TestReadModel(Func<IListener> listener, Guid testId) : base("Balance", listener)
        {
            EventStream.Subscribe<AddTestData>(this);
            Start<TestDataUtility>(testId);
        }

        private int _totalCount;

        public void Handle(AddTestData message)
        {
            _totalCount += (int) message.Value;
            InformUser();
        }

        private void InformUser()
        {
            Console.WriteLine($"Total Count = {_totalCount}");
        }
    }
}
