using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLayer.Entities.Built
{
    
    public class BuiltAchievements
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string uid { get; set; }
        [OneToOne(CascadeOperations=CascadeOperation.All)]
        public BuiltAchievementsImage image { get; set; }
        public string info { get; set; }
        public string name { get; set; }
        public string short_info { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public string deleted_at { get; set; }
    }

    public class BuiltAchievementsImage
    {

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [ForeignKey(typeof(BuiltAchievements))]
        public int BuiltAchievementsId { get; set; }

        [OneToOne]
        public BuiltAchievements BuiltAchievements { get; set; }

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
