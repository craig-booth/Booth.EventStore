using System;
using System.Collections.Generic;
using System.Text;

namespace Booth.EventStore
{
    public interface ITrackedEntityFactory<T> where T : ITrackedEntity
    {
        T Create(Guid id, string storedEntityType);
    }

    public class DefaultTrackedEntityFactory<T> : ITrackedEntityFactory<T> where T : ITrackedEntity
    {
        public T Create(Guid id, string storedEntityType)
        {
            var entity = (T)Activator.CreateInstance(typeof(T), new Object[] { id });           

            return entity;
        }
    }
}
