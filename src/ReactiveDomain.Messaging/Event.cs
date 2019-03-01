﻿using Newtonsoft.Json;
using ReactiveDomain.Messaging.Messages;

namespace ReactiveDomain.Messaging {
    public class Event : CorrelatedMessage, IEvent  {
        protected ushort Version = 1;
       
        protected Event(CorrelatedMessage source):base(new CorrelationId(source),new SourceId(source)){}
        [JsonConstructor]
        protected Event(CorrelationId correlationId, SourceId sourceId):base(correlationId, sourceId) { }
    }
}
