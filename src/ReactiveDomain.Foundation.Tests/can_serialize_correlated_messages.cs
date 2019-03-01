﻿using ReactiveDomain.Messaging;
using ReactiveDomain.Testing;
using System;
using Xunit;

namespace ReactiveDomain.Foundation.Tests {

    // ReSharper disable once InconsistentNaming
    public class when_serializing_correlated_messages {
        [Fact]
        public void can_use_json_message_serializer() {
           
            var evt = new TestEvent(CorrelatedMessage.NewRoot());
            var evt2 = new TestEvent(evt);

            var serializer = new JsonMessageSerializer();

            var data = serializer.Serialize(evt);
            var data2 = serializer.Serialize(evt2);

            var dEvent = (TestEvent)serializer.Deserialize(data);
            var dEvent2 = (TestEvent)serializer.Deserialize(data2);

            Assert.Equal(evt.MsgId,dEvent.MsgId);
            Assert.Equal(evt.SourceId,dEvent.SourceId);
            Assert.Equal(evt.CorrelationId,dEvent.CorrelationId);

            Assert.Equal(evt2.MsgId,dEvent2.MsgId);
            Assert.Equal(evt2.SourceId,dEvent2.SourceId);
            Assert.Equal(evt2.CorrelationId,dEvent2.CorrelationId);
        }
    }
    public class testMsg : Message
    {
        public readonly long Number;
        public readonly string Name;
        public readonly DateTime Time;

        public testMsg(
            long number,
            string name,
            DateTime time
            )
        {
            Number = number;
            Name = name;
            Time = time;
        }
    }
    public class when_serializing_messages
    {
        
        [Fact]
        public void can_use_json_message_serializer()
        {
            var num = 5;
            var name = "adadasdasdad";
            var time = DateTime.Now;
            var msg = new testMsg(num,name,time);

            var serializer = new JsonMessageSerializer();

            var data = serializer.Serialize(msg);;

            var dMsg = (testMsg)serializer.Deserialize(data);

            Assert.Equal(msg.MsgId, dMsg.MsgId);
            Assert.Equal(num, dMsg.Number);
            Assert.Equal(name, dMsg.Name);
            Assert.Equal(time, dMsg.Time);
        }
    }
}
