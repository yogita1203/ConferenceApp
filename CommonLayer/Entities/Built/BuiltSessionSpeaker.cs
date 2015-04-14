using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLayer.Entities.Built
{
    public class BuiltSessionSpeaker
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [ForeignKey(typeof(BuiltSession))]
        public int BuiltSessionId { get; set; }

        [ManyToOne]
        public BuiltSession BuiltSession { get; set; }
        public string uid { get; set; }
        public string user_ref { get; set; }
        public string full_name { get; set; }
        public string job_title { get; set; }
        public string company { get; set; }
        public string roles { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public string deleted_at { get; set; }
    }
}
