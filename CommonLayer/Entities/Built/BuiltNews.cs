using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLayer.Entities.Built
{
    public class BuiltNews
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string uid { get; set; }
        
        [OneToOne(CascadeOperations = CascadeOperation.All)]
        public CoverImageFile cover_image { get; set; }
        //public string description { get; set; }
        public string desc { get; set; }
        public string title { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.All)]
        public BuiltNewsLink link { get; set; }

        public string published_date { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public string deleted_at { get; set; }
    }

    public class BuiltNewsLink
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [ForeignKey(typeof(BuiltNews))]
        public int BuiltNewsId { get; set; }

        [OneToOne]
        public BuiltNews BuiltNews { get; set; }

        public string href { get; set; }
        public string title { get; set; }
    }

    public class CoverImageFile
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [ForeignKey(typeof(BuiltNews))]
        public int BuiltNewsId { get; set; }

        [OneToOne]
        public BuiltNews BuiltNews { get; set; }

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
