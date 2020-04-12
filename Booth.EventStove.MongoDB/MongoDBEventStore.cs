using System;
using System.Collections.Generic;
using System.Text;

using MongoDB.Driver;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;

using Booth.Common;

namespace Booth.EventStore.MongoDB
{
    public class MongodbEventStore : IEventStore
    {
        private readonly MongoClient _MongoClient;
        private readonly IMongoDatabase _Database;

        public MongodbEventStore(string connectionString, string database)
        {
            _MongoClient = new MongoClient(connectionString);
            _Database = _MongoClient.GetDatabase(database);

            EventStoreSerializers.Register();
        }
        public MongodbEventStore(IMongoDatabase database)
        {
            _Database = database;

            EventStoreSerializers.Register();
        }

        public IEventStream GetEventStream(string name)
        {
            return GetEventStream<object>(name);
        }

        public IEventStream<T> GetEventStream<T>(string name)
        {
            var collection = _Database.GetCollection<StoredEntity>(name);

            var eventStream = new MongodbEventStream<T>(collection);
            return (IEventStream<T>)eventStream;
        }
    }

    static class EventStoreSerializers
    {
        private static bool _Registered = false;

        public static void Register()
        {
            if (_Registered)
                return;

            _Registered = true;

            BsonSerializer.RegisterSerializer(typeof(Date), new DateSerializer());
            var conventionPack = new ConventionPack()
            {
                new IgnoreExtraElementsConvention(true),
            };
            ConventionRegistry.Register("Booth.EventStore.StoredEntity", conventionPack, t => (t == typeof(StoredEntity)) || t.IsSubclassOf(typeof(Event)));


            var eventTypes = typeof(Event).GetSubclassesOf(true);
            foreach (var eventType in eventTypes)
                BsonClassMap.LookupClassMap(eventType);
        }
    } 
}
