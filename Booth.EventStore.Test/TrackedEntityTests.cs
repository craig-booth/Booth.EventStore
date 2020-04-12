using System;
using System.Linq;

using NUnit.Framework;
using Moq;

using Booth.EventStore;
using System.Collections.Generic;

namespace Booth.EventStore.Test
{
    class TrackedEntityTests
    {

        [TestCase]
        public void FetchSingleEvents()
        {
            var entity = new TrackedEntityTestClass();

            var @event = new EventTestClass(Guid.NewGuid(), 0);
            entity.AddEvent(@event);

            var events = entity.FetchEvents();
            Assert.That(events, Is.EqualTo(new Event[] { @event } ));
        }

        [TestCase]
        public void FetchMultpleEvents()
        {
            var entity = new TrackedEntityTestClass();

            var @event1 = new EventTestClass(Guid.NewGuid(), 0);
            entity.AddEvent(@event1);

            var @event2 = new EventTestClass(Guid.NewGuid(), 0);
            entity.AddEvent(@event2);

            var events = entity.FetchEvents();
            Assert.That(events, Is.EqualTo(new Event[] { @event1, @event2 }));
        }

        [TestCase]
        public void FetchNoEvents()
        {
            var entity = new TrackedEntityTestClass();

            var events = entity.FetchEvents();
            Assert.That(events, Is.Empty);
        }

        [TestCase]
        public void ApplyEvents()
        {
            var mockRepository = new MockRepository(MockBehavior.Strict);

            var entity = mockRepository.Create<TrackedEntityTestClass>(MockBehavior.Loose);

            var @event1 = new EventTestClass(Guid.NewGuid(), 0);
            var @event2 = new EventTestClass(Guid.NewGuid(), 0);

            entity.Object.ApplyEvents(new Event[] { @event1, @event2 });

            entity.Verify(x => x.Apply(@event1), Times.Once);
            entity.Verify(x => x.Apply(@event2), Times.Once);

            mockRepository.Verify();
        }

        [TestCase]
        public void ApplyUnkownEventType()
        {
            var entity = new TrackedEntityTestClass();

            var @event = new EventTestClass2(Guid.NewGuid(), 0);

            Assert.That(() => entity.ApplyEvents(new Event[] { @event }), Throws.TypeOf(typeof(NotSupportedException)));
        }

    }

    public class TrackedEntityTestClass : TrackedEntity
    {
        public TrackedEntityTestClass() : base(Guid.NewGuid()) { }

        public TrackedEntityTestClass(Guid id) : base(id) { }

        public void AddEvent(Event @event)
        {
            PublishEvent(@event);
        }

        public virtual void Apply(EventTestClass @event) { }
    }

    public class TrackedEntityWithPropertiesTestClass : TrackedEntityTestClass, IEntityProperties
    {
        public TrackedEntityWithPropertiesTestClass() : base(Guid.NewGuid()) { }

        public TrackedEntityWithPropertiesTestClass(Guid id) : base(id) { }

        public Dictionary<string, string> Properties = new Dictionary<string, string>();
        public IDictionary<string, string> GetProperties()
        {
            return Properties;
        }
    }

    public class EventTestClass : Event
    {
        public EventTestClass(Guid id, int version) : base(id, version) { }
    }

    public class EventTestClass2 : Event
    {
        public EventTestClass2(Guid id, int version) : base(id, version) { }
    }
}
