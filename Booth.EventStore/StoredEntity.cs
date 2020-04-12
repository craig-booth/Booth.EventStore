using System;
using System.Collections.Generic;
using System.Text;

namespace Booth.EventStore
{
    public class StoredEntity
    {
        public Guid EntityId { get; set; }
        public string Type { get; set; }
        public int CurrentVersion { get; set; }

        public Dictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();

        public List<Event> Events { get; set; } = new List<Event>();
    }
}
