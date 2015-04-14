using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLayer.Entities.Built
{
    public class BuiltMosconeCenter
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string uid { get; set; }
        public string address { get; set; }
        public string info { get; set; }
        public string name { get; set; }

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<VenuePicFile> venue_pic { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public string deleted_at { get; set; }
    }

    public class VenuePicFile
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [ForeignKey(typeof(BuiltMosconeCenter))]
        public int BuiltMosconeCenterId { get; set; }

        [ManyToOne]
        public BuiltMosconeCenter BuiltMosconeCenter { get; set; }

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
