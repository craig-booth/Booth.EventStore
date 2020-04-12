using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Booth.EventStore
{

    public interface IEventList
    {
        void Add(Event @event);
        bool EventsAvailable { get; }
        IEnumerable<Event> Fetch();
    }

    public class EventList : IEventList
    {
        private Queue<Event> _Events = new Queue<Event>();

        public void Add(Event @event)
        {
            _Events.Enqueue(@event);
        }

        public bool EventsAvailable
        {
            get { return _Events.Count > 0; }
        }
        
        public IEnumerable<Event> Fetch()
        {
            var eventsToReturn = new List<Event>();

            while (_Events.Count > 0)
                eventsToReturn.Add(_Events.Dequeue());

            return eventsToReturn;
        }
    }
}
