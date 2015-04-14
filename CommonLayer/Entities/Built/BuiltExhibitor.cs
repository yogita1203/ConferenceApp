using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLayer.Entities.Built
{
    public class BuiltExhibitor
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string uid { get; set; }
        public string exhibitor_id { get; set; }
        public string name { get; set; }
        public string status { get; set; }
        public string type { get; set; }
        public string description { get; set; }
        public string url { get; set; }
        public string email { get; set; }
        public string company_description { get; set; }
        public string booth { get; set; }
        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<BuiltExhibitorFile> exhibitor_file { get; set; }
        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<BuiltParticipant> participant { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public string deleted_at { get; set; }
    }

    public class BuiltExhibitorFile
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [ForeignKey(typeof(BuiltExhibitor))]
        public int BuiltExhibitorId { get; set; }

        [ManyToOne]
        public BuiltExhibitor BuiltExhibitor { get; set; }
        public string type { get; set; }
        //public string display_name { get; set; }
        //public string description { get; set; }
        public string url { get; set; }
        //public string created_at { get; set; }
        //public string updated_at { get; set; }
        //public string deleted_at { get; set; }
    }

    public class BuiltParticipant
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [ForeignKey(typeof(BuiltExhibitor))]
        public int BuiltExhibitorId { get; set; }

        [ManyToOne]
        public BuiltExhibitor BuiltExhibitor { get; set; }
        public string role { get; set; }
        public string user_ref { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string email { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public string deleted_at { get; set; }
    }
}
