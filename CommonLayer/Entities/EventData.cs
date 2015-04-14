using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLayer.Entities
{
    public class EventData
    {
        public string previous_event_uid { get; set; }
        public Dictionary<string, object> properties { get; set; }
        public string uid { get; set; }
    }
}
