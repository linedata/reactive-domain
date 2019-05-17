using System;
using ReactiveDomain;
using ReactiveDomain.Messaging;

namespace ReactiveDomain.Persistence.Samples
{
    public class TestDataUtility : EventDrivenStateMachine {
        private long _totalCount;

        public TestDataUtility() { Setup(); }

        public TestDataUtility(Guid id) : this() => Raise(new TestDataCreated(id));

        public void Setup() {
            Register<TestDataCreated>(evt => Id = evt.Id);
            Register<AddValue>(Apply);
        }

        private void Apply(AddValue testEvent) => _totalCount += testEvent.Value;

        public void AddValue(int value) => Raise(new AddValue(value));
    }

    public class AddTestData : Message {
        public int Value;

        public AddTestData(int value) => Value = value;
    }

    public class TestDataCreated : Message {
        public readonly Guid Id;

        public TestDataCreated(Guid id) => Id = id;
    }

    public class AddValue : Message {
        public int Value;

        public AddValue(int value) => Value = value;
    }
}
