using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLayer.Entities.Built
{
    public class BuiltSync
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string uid { get; set; }
        public string exhibitor_last_synced { get; set; }
        public bool is_exhibitor_syncing { get; set; }
        public string speaker_last_synced { get; set; }
        public bool is_speaker_syncing { get; set; }
        public string session_last_synced { get; set; }
        public bool is_session_syncing { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public string deleted_at { get; set; }
    }
}
