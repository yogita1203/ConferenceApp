using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLayer.Entities.Built
{
    public class BuiltIntro
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string text_color { get; set; }
        public string bg_color { get; set; }
        [OneToOne(CascadeOperations=CascadeOperation.All)]
        public BuiltIntroBgImage bg_image { get; set; }
        public string desc { get; set; }
        public string uid { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public string deleted_at { get; set; }
    }

    public class BuiltIntroBgImage
    {

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [ForeignKey(typeof(BuiltIntro))]
        public int BuiltIntroId { get; set; }

        [OneToOne]
        public BuiltIntro BuiltIntro { get; set; }
        //public string app_user_object_uid { get; set; }
        //public string content_type { get; set; }
        //public string file_size { get; set; }
        //[Ignore]
        //public string[] tags { get; set; }
        //public string sqlite_tags { get; set; }
        //public string uid { get; set; }
        public string url { get; set; }
        //public string filename { get; set; }
    }
}
