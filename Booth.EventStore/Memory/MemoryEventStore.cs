using System;
using System.Collections.Generic;
using System.Text;

namespace Booth.EventStore.Memory
{
    public class MemoryEventStore : IEventStore
    {
        private readonly Dictionary<string, object> _EventStreams;

        public MemoryEventStore()
        {
            _EventStreams = new Dictionary<string, object>();
        }

        public IEventStream GetEventStream(string collection)
        {
            return GetEventStream<object>(collection);
        }

        public IEventStream<T> GetEventStream<T>(string collection)
        {
            if (_EventStreams.ContainsKey(collection))
                return (IEventStream<T>)_EventStreams[collection];
            else
            {
                var eventStream = new MemoryEventStream<T>(collection);
                _EventStreams.Add(collection, eventStream);

                return eventStream;
            }
        }
 
    }
}
