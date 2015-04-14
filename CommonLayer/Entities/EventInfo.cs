using SQLite.Net.Attributes;
using System.Collections.Generic;

namespace CommonLayer.Entities
{
    public class EventInformation
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string uid { get; set; }
        public string timestamp { get; set; }
        public string previous_event_uid { get; set; }

        [Ignore]
        public Dictionary<string, object> Extras { get; set; }
    }
}
