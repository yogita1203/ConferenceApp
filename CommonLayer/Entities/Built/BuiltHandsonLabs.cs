using Newtonsoft.Json;
using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLayer.Entities.Built
{
    public class BuiltHandsonLabs
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string uid { get; set; }
        public string hol_category { get; set; }
        public string session_id { get; set; }
        public string title { get; set; }
        [JsonProperty("abstract")]
        public string _abstract { get; set; }

        [Ignore]
        public List<string> speaker { get; set; }
        public string speaker_separated { get; set; }

        public string duration { get; set; }
        public string capacity { get; set; }
        public string hashtag { get; set; }
        public Int32 sequence { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public string deleted_at { get; set; }
    }
}
