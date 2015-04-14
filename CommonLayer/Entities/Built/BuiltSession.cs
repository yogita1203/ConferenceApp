using Newtonsoft.Json;
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLayer.Entities.Built
{
    public class BuiltSession
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string uid { get; set; }
        public string abbreviation { get; set; }
        public string session_id { get; set; }
        public string title { get; set; }
        [JsonProperty("abstract")]
        public string _abstract { get; set; }
        public Int32 times_offered { get; set; }
        public Int32 capacity { get; set; }
        public string type { get; set; }
        public string track { get; set; }
        public string status { get; set; }
        public Int32 length { get; set; }
        public string session_created { get; set; }
        public string modified { get; set; }
        public string session_published { get; set; }
        public string public_view_private_schedule { get; set; }
        public string private_view_private_schedule { get; set; }
        public string cannot_view_or_schedule { get; set; }
        [Ignore]
        public List<string> role { get; set; }
        public string role_separated { get; set; }
        public string sddc_topics { get; set; }
        [Ignore]
        public List<string> skill_level { get; set; }
        public string skill_level_separated { get; set; }
        [Ignore]
        public List<string> solutions { get; set; }
        public string solutions_separated { get; set; }
        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<BuiltSessionFile> file { get; set; }
        public string hybrid_cloud_topics { get; set; }
        public string mobility_topics { get; set; }
        public string vmware_value_network_topics { get; set; }
        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<BuiltSessionTime> session_time { get; set; }
        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<BuiltSessionSpeaker> speaker { get; set; }
        public Int32 isPopular { get; set; }
        [Ignore]
        public List<string> sub_track { get; set; }
        public string sub_track_separated { get; set; }
        [Ignore]
        public List<string> audience { get; set; }
        public string audience_separated { get; set; }
        public string product_and_topic { get; set; }
        [Ignore]
        public List<string> program_location { get; set; }
        public string program_location_separated { get; set; }
        public string technical_level { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public string deleted_at { get; set; }
        [Ignore]
        public List<string> market_segment { get; set; }
        public string market_segment_separated { get; set; }

        [Ignore]
        public List<string> competency { get; set; }
        public string competency_separated { get; set; }
    }
}
