using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLayer.Entities.Built
{
    public class BuiltSpeakerSession
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [ForeignKey(typeof(BuiltSpeaker))]
        public int BuiltSpeakerId { get; set; }

        [ManyToOne]
        public BuiltSpeaker BuiltSpeaker { get; set; }
        public string uid { get; set; }
        public string session_id { get; set; }
        public string abbreviation { get; set; }
        public string title { get; set; }
        public string session_published { get; set; }
        public string speaker_role { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public string deleted_at { get; set; }
    }
}
