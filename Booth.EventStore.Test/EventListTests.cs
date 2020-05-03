using System;

using Xunit;
using FluentAssertions;
using FluentAssertions.Execution;

using Booth.Common;
using Booth.EventStore;

namespace Booth.EventStore.Test
{
    public class EventListTests
    {

        [Fact]
        public void EventsAvailable()
        {
            var eventList = new EventList();

            var @event = new EventTestClass(Guid.NewGuid(), 0);

            eventList.Add(@event);

            eventList.EventsAvailable.Should().BeTrue();
        }

        [Fact]
        public void EventsAvailableOnEmptyList()
        {
            var eventList = new EventList();

            eventList.EventsAvailable.Should().BeFalse();
        }

        [Fact]
        public void FetchSingleEvent()
        {
            var eventList = new EventList();

            var @event = new EventTestClass(Guid.NewGuid(), 0);

            eventList.Add(@event);

            var events = eventList.Fetch();

            using (new AssertionScope())
            {
                events.Should().HaveCount(1);
                eventList.EventsAvailable.Should().BeFalse();
            }
        }

        [Fact]
        public void FetchThreeEvents()
        {
            var eventList = new EventList();

            var @event = new EventTestClass(Guid.NewGuid(), 0);

            eventList.Add(@event);
            eventList.Add(@event);
            eventList.Add(@event);

            var events = eventList.Fetch();

            using (new AssertionScope())
            {
                events.Should().HaveCount(3);
                eventList.EventsAvailable.Should().BeFalse();
            }
        }

        [Fact]
        public void FetchNoEventsAvailable()
        {
            var eventList = new EventList();

            var events = eventList.Fetch();

            events.Should().BeEmpty();
        }

        [Fact]
        public void FetchCalledMultipleTimes()
        {
            var eventList = new EventList();

            var @event = new EventTestClass(Guid.NewGuid(), 0);

            eventList.Add(@event);
            eventList.Add(@event);
            eventList.Add(@event);

            var events = eventList.Fetch();
            var events2 = eventList.Fetch();

            using (new AssertionScope())
            {
                events.Should().HaveCount(3);
                events2.Should().BeEmpty();
            }
        }
    }
}
