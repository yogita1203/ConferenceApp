using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLayer.Entities.Built
{
    public class BuiltSessionTime
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [ForeignKey(typeof(BuiltSession))]
        public int BuiltSessionId { get; set; }

        [ManyToOne]
        public BuiltSession BuiltSession { get; set; }
        public string uid { get; set; }
        public string session_time_id { get; set; }
        public string room { get; set; }
        public string date { get; set; }
        public string time { get; set; }
        public string start_date_time { get; set; }
        public string end_date_time { get; set; }
        public string capacity { get; set; }
        public string registered { get; set; }
        public string length { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public string deleted_at { get; set; }
    }
}
