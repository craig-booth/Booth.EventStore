using System;
using System.Linq;

using NUnit.Framework;
using Moq;

using Booth.Common;
using Booth.EventStore;
using FluentAssertions;

namespace Booth.EventStore.Test
{
    class RepositoryTests
    {

        [TestCase]
        public void GetWithMatchingId()
        {          
            var mockRepository = new MockRepository(MockBehavior.Strict);

            var id = Guid.NewGuid();

            var eventStream = mockRepository.Create<IEventStream<TrackedEntityTestClass>>();
            eventStream.Setup(x => x.Get(id)).Returns(new StoredEntity() { EntityId = id, Type = "" });

            var entityFactory = mockRepository.Create<ITrackedEntityFactory<TrackedEntityTestClass>>();
            entityFactory.Setup(x => x.Create(id, "")).Returns(new TrackedEntityTestClass(id));
            var repository = new Repository<TrackedEntityTestClass>(eventStream.Object, entityFactory.Object);
            
            var entity = repository.Get(id);

            entity.Id.Should().Be(id);

            mockRepository.Verify();
        }

        [TestCase]
        public void GetWithoutMatchingId()
        {
            var mockRepository = new MockRepository(MockBehavior.Strict);

            var id = Guid.NewGuid();

            var eventStream = mockRepository.Create<IEventStream<TrackedEntityTestClass>>();
            eventStream.Setup(x => x.Get(id)).Returns<StoredEntity>(null);

            var entityFactory = mockRepository.Create<ITrackedEntityFactory<TrackedEntityTestClass>>();
            entityFactory.Setup(x => x.Create(id, "")).Returns(new TrackedEntityTestClass(id));
            var repository = new Repository<TrackedEntityTestClass>(eventStream.Object, entityFactory.Object);

            var entity = repository.Get(id);

            entity.Should().BeNull();

            mockRepository.Verify();
        }

        [TestCase]
        public void GetAll()
        {
            var mockRepository = new MockRepository(MockBehavior.Strict);

            var id1 = Guid.NewGuid();
            var id2 = Guid.NewGuid();
            var id3 = Guid.NewGuid();

            var storedEntities = new StoredEntity[]
            {
                new StoredEntity() { EntityId = id1, Type = "" },
                new StoredEntity() { EntityId = id2, Type = "" },
                new StoredEntity() { EntityId = id3, Type = "" }
            };

            var eventStream = mockRepository.Create<IEventStream<TrackedEntityTestClass>>();
            eventStream.Setup(x => x.GetAll()).Returns(storedEntities);

            var entityFactory = mockRepository.Create<ITrackedEntityFactory<TrackedEntityTestClass>>();
            entityFactory.Setup(x => x.Create(id1, "")).Returns(new TrackedEntityTestClass(id1));
            entityFactory.Setup(x => x.Create(id2, "")).Returns(new TrackedEntityTestClass(id2));
            entityFactory.Setup(x => x.Create(id3, "")).Returns(new TrackedEntityTestClass(id3));
            var repository = new Repository<TrackedEntityTestClass>(eventStream.Object, entityFactory.Object);


            var entities = repository.All();

            entities.Should().BeEquivalentTo(new object[] { new { Id = id1 }, new { Id = id2 }, new { Id = id3 } });

            mockRepository.Verify();
        }

        [TestCase]
        public void GetAllReturningEmptyList()
        {
            var mockRepository = new MockRepository(MockBehavior.Strict);

            var eventStream = mockRepository.Create<IEventStream<TrackedEntityTestClass>>();
            eventStream.Setup(x => x.GetAll()).Returns(new StoredEntity[] { });

            var entityFactory = mockRepository.Create<ITrackedEntityFactory<TrackedEntityTestClass>>();
            var repository = new Repository<TrackedEntityTestClass>(eventStream.Object, entityFactory.Object);

            var entities = repository.All();

            entities.Should().BeEmpty();

            mockRepository.Verify(); 
        }

        [TestCase]
        public void AddEntity()
        {
            var mockRepository = new MockRepository(MockBehavior.Strict);

            var entityId = Guid.NewGuid();
            var events = new Event[]
            {
                new EventTestClass(Guid.NewGuid(), 0)
            };
            var trackedEntity = new TrackedEntityTestClass(entityId);
            trackedEntity.AddEvent(events[0]);

            var eventStream = mockRepository.Create<IEventStream<TrackedEntityTestClass>>();
            eventStream.Setup(x => x.Add(entityId, "TrackedEntityTestClass", events));

            var entityFactory = mockRepository.Create<ITrackedEntityFactory<TrackedEntityTestClass>>();

            var repository = new Repository<TrackedEntityTestClass>(eventStream.Object, entityFactory.Object);

            repository.Add(trackedEntity);

            mockRepository.Verify();
        }

        [TestCase]
        public void AddEntityWithProperties()
        {
            var mockRepository = new MockRepository(MockBehavior.Strict);

            var entityId = Guid.NewGuid();
            var events = new Event[]
            {
                new EventTestClass(Guid.NewGuid(), 0)
            };
            var trackedEntity = new TrackedEntityWithPropertiesTestClass(entityId);
            trackedEntity.Properties.Add("Name1", "Value1");
            trackedEntity.AddEvent(events[0]);
  
            var eventStream = mockRepository.Create<IEventStream<TrackedEntityWithPropertiesTestClass>>();
            eventStream.Setup(x => x.Add(entityId, "TrackedEntityWithPropertiesTestClass", trackedEntity.Properties, events));

            var entityFactory = mockRepository.Create<ITrackedEntityFactory<TrackedEntityWithPropertiesTestClass>>();

            var repository = new Repository<TrackedEntityWithPropertiesTestClass>(eventStream.Object, entityFactory.Object);

            repository.Add(trackedEntity);

            mockRepository.Verify();
        }

        [TestCase]
        public void UpdateExistingEntity()
        {
            var mockRepository = new MockRepository(MockBehavior.Strict);

            var entityId = Guid.NewGuid();
            var events = new Event[]
            {
                new EventTestClass(Guid.NewGuid(), 0)
            };
            var trackedEntity = new TrackedEntityTestClass(entityId);
            trackedEntity.AddEvent(events[0]);

            var eventStream = mockRepository.Create<IEventStream<TrackedEntityTestClass>>();
            eventStream.Setup(x => x.AppendEvents(entityId, events));

            var entityFactory = mockRepository.Create<ITrackedEntityFactory<TrackedEntityTestClass>>();

            var repository = new Repository<TrackedEntityTestClass>(eventStream.Object, entityFactory.Object);

            repository.Update(trackedEntity);

            mockRepository.Verify();
        }

        [TestCase]
        public void FindFirstOnlyMatchingEntity()
        {
            var mockRepository = new MockRepository(MockBehavior.Strict);

            var id = Guid.NewGuid();

            var eventStream = mockRepository.Create<IEventStream<TrackedEntityTestClass>>();
            eventStream.Setup(x => x.FindFirst("Property", "Value")).Returns(new StoredEntity() { EntityId = id, Type = "" });

            var entityFactory = mockRepository.Create<ITrackedEntityFactory<TrackedEntityTestClass>>();
            entityFactory.Setup(x => x.Create(id, "")).Returns(new TrackedEntityTestClass(id));

            var repository = new Repository<TrackedEntityTestClass>(eventStream.Object, entityFactory.Object);

            var entity = repository.FindFirst("Property", "Value");

            entity.Id.Should().Be(id);

            mockRepository.Verify();
        }


        [TestCase]
        public void FindFirstNoMatchingEntities()
        {
            var mockRepository = new MockRepository(MockBehavior.Strict);

            var id = Guid.NewGuid();

            var eventStream = mockRepository.Create<IEventStream<TrackedEntityTestClass>>();
            eventStream.Setup(x => x.FindFirst("Property", "Value")).Returns<StoredEntity>(null);

            var entityFactory = mockRepository.Create<ITrackedEntityFactory<TrackedEntityTestClass>>();

            var repository = new Repository<TrackedEntityTestClass>(eventStream.Object, entityFactory.Object);

            var entity = repository.FindFirst("Property", "Value");

            entity.Should().BeNull();

            mockRepository.Verify();
        }

        [TestCase]
        public void FindSingleMatchingEntity()
        {
            var mockRepository = new MockRepository(MockBehavior.Strict);

            var id = Guid.NewGuid();

            var eventStream = mockRepository.Create<IEventStream<TrackedEntityTestClass>>();
            eventStream.Setup(x => x.Find("Property", "Value")).Returns(new StoredEntity[] { new StoredEntity() { EntityId = id, Type = "" } });

            var entityFactory = mockRepository.Create<ITrackedEntityFactory<TrackedEntityTestClass>>();
            entityFactory.Setup(x => x.Create(id, "")).Returns(new TrackedEntityTestClass(id));

            var repository = new Repository<TrackedEntityTestClass>(eventStream.Object, entityFactory.Object);

            var entities = repository.Find("Property", "Value");

            entities.Should().HaveCount(1);

            mockRepository.Verify();
        }

        [TestCase]
        public void FindMultipleMatchingEntities()
        {
            var mockRepository = new MockRepository(MockBehavior.Strict);

            var id1 = Guid.NewGuid();
            var id2 = Guid.NewGuid();
            var storedEntities = new StoredEntity[]
            {
                new StoredEntity() { EntityId = id1, Type = "" },
                new StoredEntity() { EntityId = id2, Type = "" }
            };

            var eventStream = mockRepository.Create<IEventStream<TrackedEntityTestClass>>();
            eventStream.Setup(x => x.Find("Property", "Value")).Returns(storedEntities);

            var entityFactory = mockRepository.Create<ITrackedEntityFactory<TrackedEntityTestClass>>();
            entityFactory.Setup(x => x.Create(id1, "")).Returns(new TrackedEntityTestClass(id1));
            entityFactory.Setup(x => x.Create(id2, "")).Returns(new TrackedEntityTestClass(id2));

            var repository = new Repository<TrackedEntityTestClass>(eventStream.Object, entityFactory.Object);

            var entity = repository.Find("Property", "Value");

            entity.Should().HaveCount(2);

            mockRepository.Verify();
        }

        [TestCase]
        public void FindNoMatchingEntities()
        {
            var mockRepository = new MockRepository(MockBehavior.Strict);

            var eventStream = mockRepository.Create<IEventStream<TrackedEntityTestClass>>();
            eventStream.Setup(x => x.Find("Property", "Value")).Returns(new StoredEntity[] { });

            var entityFactory = mockRepository.Create<ITrackedEntityFactory<TrackedEntityTestClass>>();

            var repository = new Repository<TrackedEntityTestClass>(eventStream.Object, entityFactory.Object);

            var entities = repository.Find("Property", "Value");

            entities.Should().BeEmpty();

            mockRepository.Verify();
        }
    }
}
