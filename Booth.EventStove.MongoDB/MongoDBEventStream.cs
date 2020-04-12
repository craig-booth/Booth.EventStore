using System;
using System.Collections.Generic;
using System.Text;

using MongoDB.Driver;

using Booth.EventStore;


namespace Booth.EventStore.MongoDB
{
    class MongodbEventStream<T> :
       IEventStream,
       IEventStream<T>
    {
        private readonly IMongoCollection<StoredEntity> _Collection;

        public string Collection
        {
            get { return _Collection.CollectionNamespace.CollectionName; }
        }

        public MongodbEventStream(IMongoCollection<StoredEntity> collection)
        {
            _Collection = collection;
        }

        public StoredEntity Get(Guid entityId)
        {
            var entity = _Collection.Find(x => x.EntityId == entityId)?.Single();

            return entity;
        }

        public IEnumerable<StoredEntity> GetAll()
        {
            var result = new List<StoredEntity>();

            var entities = _Collection.Find("{}").ToList<StoredEntity>();

            return entities;
        }

        public StoredEntity FindFirst(string property, string value)
        {
            var filter = String.Format("{{Properties: {{{0}: '{1}'}}}}", property, value);
            var result = _Collection.Find(filter).Limit(1).ToList();
            if (result.Count == 0)
                return null;

            return result[0];
        }

        public IEnumerable<StoredEntity> Find(string property, string value)
        {
            var filter = String.Format("{{Properties: {{{0}: '{1}'}}}}", property, value);
            var result = _Collection.Find(filter).ToList();

            return result;
        }

        public void Add(Guid entityId, string type, IEnumerable<Event> events)
        {
            Add(entityId, type, null, events);
        }

        public void Add(Guid entityId, string type, IDictionary<string, string> properties, IEnumerable<Event> events)
        {
            var entity = new StoredEntity()
            {
                EntityId = entityId,
                Type = type
            };

            if (properties != null)
            {
                foreach (var item in properties)
                    entity.Properties.Add(item.Key, item.Value);
            }

            entity.Events.AddRange(events);

            _Collection.InsertOne(entity);
        }

        public void UpdateProperties(Guid entityId, IDictionary<string, string> properties)
        {
            _Collection.FindOneAndUpdate<StoredEntity>(x => x.EntityId == entityId,
                Builders<StoredEntity>.Update.Set<Dictionary<string, string>>(x => x.Properties, (Dictionary<string, string>)properties));
        }

        public void AppendEvent(Guid entityId, Event @event)
        {
            _Collection.FindOneAndUpdate<StoredEntity>(x => x.EntityId == entityId,
                Builders<StoredEntity>.Update.Push<Event>(x => x.Events, @event));
        }

        public void AppendEvents(Guid entityId, IEnumerable<Event> events)
        {
            _Collection.FindOneAndUpdate<StoredEntity>(x => x.EntityId == entityId,
                Builders<StoredEntity>.Update.PushEach<Event>(x => x.Events, events));
        }

    }
}
