using System;
using System.Collections.Generic;
using System.Linq;

using MongoDB.Driver;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;

using NUnit.Framework;
using Moq;

using Booth.Common;
using Booth.EventStore.MongoDB;

namespace Booth.EventStore.MongoDB.Test
{
    class EventStoreTests
    {
        [TestCase]
        public void IncludesNeededConventions()
        {
            var database = Mock.Of<IMongoDatabase>();
            var eventStore = new MongodbEventStore(database);

            var conventionPack = ConventionRegistry.Lookup(typeof(Event));

            Assert.That(conventionPack.Conventions.Any(x => x.Name == "IgnoreExtraElements"), Is.True);
        }

        [TestCase]
        public void DateSerializerRegistered()
        {
            var database = Mock.Of<IMongoDatabase>();
            var eventStore = new MongodbEventStore(database);

            var serializer = BsonSerializer.LookupSerializer<Date>();

            Assert.That(serializer, Is.TypeOf<DateSerializer>());
        }

        [TestCase]
        public void EventTypesMapped()
        {
            var database = Mock.Of<IMongoDatabase>();
            var eventStore = new MongodbEventStore(database);

            Assert.That(BsonClassMap.IsClassMapRegistered(typeof(TestEvent)), Is.True);
        }

    }

    class TestEvent: Event
    {
        public TestEvent(Guid id, int version)
            :base(id, version)
        {

        }
    }
}
