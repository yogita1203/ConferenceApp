using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLayer.Entities.Built
{
    public class BuiltConfig
    {
        //public string uid { get; set; }
        //public string event_name { get; set; }
        //public BuiltExplore explore { get; set; }
        //public BuiltConfigMenu menu { get; set; }
        //public BuiltConfigSocial social { get; set; }
        //public BuiltSponsors sponsors { get; set; }
        //public string last_synced_delta_time { get; set; }
        //public BuiltConfigImportantLinks important_links { get; set; }
        //public BuiltHOLCategoryOrder hol_order { get; set; }
        //public BuiltGame game { get; set; }
        //public string created_at { get; set; }
        //public string updated_at { get; set; }
        //public string deleted_at { get; set; }


        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string uid { get; set; }
        public string event_name { get; set; }
        [OneToOne(CascadeOperations = CascadeOperation.All)]
        public BuiltExplore explore { get; set; }
        [OneToOne(CascadeOperations = CascadeOperation.All)]
        public BuiltConfigMenu menu { get; set; }
        [OneToOne(CascadeOperations = CascadeOperation.All)]
        public BuiltConfigSocial social { get; set; }
        [OneToOne(CascadeOperations = CascadeOperation.All)]
        public BuiltSponsors sponsors { get; set; }
        public string last_synced_delta_time { get; set; }
        [OneToOne(CascadeOperations = CascadeOperation.All)]
        public BuiltConfigImportantLinks important_links { get; set; }
        [OneToOne(CascadeOperations = CascadeOperation.All)]
        public BuiltHOLCategoryOrder hol_order { get; set; }
        [OneToOne(CascadeOperations = CascadeOperation.All)]
        public BuiltGame game { get; set; }
        public string timezone { get; set; }
        public int clear_db_id { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public string deleted_at { get; set; }
    }
}
