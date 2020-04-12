using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Booth.EventStore
{
    public interface IEventStore
    {
        IEventStream GetEventStream(string collection);
        IEventStream<T> GetEventStream<T>(string collection);
    } 

}
