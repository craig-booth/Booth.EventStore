using System;

using NUnit.Framework;

using Booth.Common;
using Booth.EventStore;

namespace Booth.EventStore.Test
{
    class EventListTests
    {

        [TestCase]
        public void EventsAvailable()
        {
            var eventList = new EventList();

            var @event = new EventTestClass(Guid.NewGuid(), 0);

            eventList.Add(@event);

            Assert.That(eventList.EventsAvailable, Is.True);
        }

        [TestCase]
        public void EventsAvailableOnEmptyList()
        {
            var eventList = new EventList();

            Assert.That(eventList.EventsAvailable, Is.False);
        }

        [TestCase]
        public void FetchSingleEvent()
        {
            var eventList = new EventList();

            var @event = new EventTestClass(Guid.NewGuid(), 0);

            eventList.Add(@event);

            var events = eventList.Fetch();

            Assert.Multiple(() =>
            {
                Assert.That(events, Has.Count.EqualTo(1));
                Assert.That(eventList.EventsAvailable, Is.False);
            });
        }

        [TestCase]
        public void FetchThreeEvents()
        {
            var eventList = new EventList();

            var @event = new EventTestClass(Guid.NewGuid(), 0);

            eventList.Add(@event);
            eventList.Add(@event);
            eventList.Add(@event);

            var events = eventList.Fetch();
            Assert.Multiple(() =>
            {
                Assert.That(events, Has.Count.EqualTo(3));
                Assert.That(eventList.EventsAvailable, Is.False);
            });
        }

        [TestCase]
        public void FetchNoEventsAvailable()
        {
            var eventList = new EventList();

            var events = eventList.Fetch();

            Assert.That(events, Is.Empty);
        }

        [TestCase]
        public void FetchCalledMultipleTimes()
        {
            var eventList = new EventList();

            var @event = new EventTestClass(Guid.NewGuid(), 0);

            eventList.Add(@event);
            eventList.Add(@event);
            eventList.Add(@event);

            var events = eventList.Fetch();
            var events2 = eventList.Fetch();
            Assert.Multiple(() =>
            {
                Assert.That(events, Has.Count.EqualTo(3));
                Assert.That(events2, Is.Empty);
            });
        }
    }
}
