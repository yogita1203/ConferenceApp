using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLayer.Entities.Built
{
    public class BuiltExplore
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [ForeignKey(typeof(BuiltConfig))]
        public int BuiltConfigId { get; set; }

        [OneToOne]
        public BuiltConfig BuiltConfig { get; set; }

        public string uid { get; set; }
        public bool show_banner { get; set; }
        public bool show_recommended_sessions { get; set; }
        public bool show_tracks { get; set; }
        public bool show_sponsors { get; set; }
        public bool show_leaderboard { get; set; }
        
        [Ignore]
        public List<string> banner_details { get; set; }
        public string banner_details_separated { get; set; }
        public bool show_sessions { get; set; }
        public bool enable_analytics { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public string deleted_at { get; set; }
    }
}
