using System;
using System.Collections.Generic;
using System.Linq;

namespace Booth.EventStore.Memory
{
    class MemoryEventStream<T> :
        IEventStream,
        IEventStream<T>
    {
        public string Collection { get; private set; }
        private Dictionary<Guid, StoredEntity> _Entities = new Dictionary<Guid, StoredEntity>();

        public MemoryEventStream(string collection)
        {
            Collection = collection;
        }

        public StoredEntity Get(Guid entityId)
        {
            if (_Entities.ContainsKey(entityId))
                return _Entities[entityId];
            else
                return null;
        }

        public IEnumerable<StoredEntity> GetAll()
        {
            return _Entities.Values;
        }

        public StoredEntity FindFirst(string property, string value)
        {
            var item = _Entities.FirstOrDefault(x => x.Value.Properties[property] == value);
            return item.Value;
        }

        public IEnumerable<StoredEntity> Find(string property, string value)
        {
            var item = _Entities.Where(x => x.Value.Properties[property] == value);
            return item.Select(x => x.Value);
        }

        public void Add(Guid entityId, string type, IEnumerable<Event> events)
        {
            var entity = new StoredEntity()
            {
                EntityId = entityId,
                Type = type,
                CurrentVersion = 0
            };

            entity.Events.AddRange(events);

            _Entities.Add(entityId, entity);
        }

        public void Add(Guid entityId, string type, IDictionary<string, string> properties, IEnumerable<Event> events)
        {
            var entity = new StoredEntity()
            {
                EntityId = entityId,
                Type = type,
                CurrentVersion = 0
            };

            foreach (var item in properties)
                entity.Properties.Add(item.Key, item.Value);

            entity.Events.AddRange(events);

            _Entities.Add(entityId, entity);
        }

        public void UpdateProperties(Guid entityId, IDictionary<string, string> properties)
        {
            if (_Entities.ContainsKey(entityId))
            {
                var entity = _Entities[entityId];
                foreach (var item in properties)
                    entity.Properties[item.Key] = item.Value;
            }
            else
                throw new KeyNotFoundException();
        }

        public void AppendEvent(Guid entityId, Event @event)
        {
            if (_Entities.ContainsKey(entityId))
            {
                var entity = _Entities[entityId];
                entity.Events.Add(@event);
            }
            else
                throw new KeyNotFoundException();
        }

        public void AppendEvents(Guid entityId, IEnumerable<Event> events)
        {
            if (_Entities.ContainsKey(entityId))
            {
                var entity = _Entities[entityId];
                entity.Events.AddRange(events);
            }
            else
                throw new KeyNotFoundException();
        }
    }
}
