using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLayer.Entities.Built
{
    public class BuiltSpeaker
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string uid { get; set; }
        public string user_ref { get; set; }
        public string bio { get; set; }
        public string company_name { get; set; }
        public string email { get; set; }
        public string first_name { get; set; }
        public string full_name { get; set; }
        public string job_title { get; set; }
        public string last_name { get; set; }
        public string photo_url { get; set; }
        public string roles { get; set; }
        public string thumb_url { get; set; }

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<BuiltSpeakerSession> session { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public string deleted_at { get; set; }

    }
}
