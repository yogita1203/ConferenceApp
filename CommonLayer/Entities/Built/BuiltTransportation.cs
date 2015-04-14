
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLayer.Entities.Built
{
    public class BuiltTransportation
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string uid { get; set; }
        public string android_link { get; set; }
        public string category { get; set; }
        
        [OneToOne(CascadeOperations = CascadeOperation.All)]
        public TransportationIcon icon { get; set; }
        public string ios_link { get; set; }
        public string name { get; set; }
        public string short_desc { get; set; }
        public string app_store_link { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public string deleted_at { get; set; }

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<TransLinkGroup> link_group { get; set; }
    }

    public class TransLinkGroup
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [ForeignKey(typeof(BuiltTransportation))]
        public int BuiltTransportationId { get; set; }

        [ManyToOne]
        public BuiltTransportation BuiltTransportation { get; set; }

        public string link { get; set; }
        public string technology { get; set; }
    }

    public class TransportationIcon
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [ForeignKey(typeof(BuiltTransportation))]
        public int BuiltTransportationId { get; set; }
        [OneToOne]
        public BuiltTransportation BuiltTransportation { get; set; }
        //public string app_user_object_uid { get; set; }
        //public string content_type { get; set; }
        //public string file_size { get; set; }
        //public string filename { get; set; }

        //[Ignore]
        //public string[] tags { get; set; }
        //public string sqlitetags { get; set; }
        //public string uid { get; set; }
        public string url { get; set; }
    }
}
