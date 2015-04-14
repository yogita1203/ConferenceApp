using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLayer.Entities.Built
{
    public class BuiltSFFoodNDrink
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string uid { get; set; }
        public string address { get; set; }
        public string android_link { get; set; }
        public string category { get; set; }
        [OneToOne(CascadeOperations = CascadeOperation.All)]
        public BuiltSFFoodNDrinkImage icon { get; set; }
        public string venue_name { get; set; }
        public string app_store_link { get; set; }
        public string ios_link { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public string deleted_at { get; set; }

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<FDLinkGroup> link_group { get; set; }
    }

    public class FDLinkGroup
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [ForeignKey(typeof(BuiltSFFoodNDrink))]
        public int BuiltSFFoodNDrinkId { get; set; }

        [ManyToOne]
        public BuiltSFFoodNDrink BuiltSFFoodNDrink { get; set; }

        public string link { get; set; }
        public string technology { get; set; }
    }

    public class BuiltSFFoodNDrinkImage
    {

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [ForeignKey(typeof(BuiltSFFoodNDrink))]
        public int BuiltAchievements { get; set; }

        [OneToOne]
        public BuiltSFFoodNDrink BuiltSFFoodNDrink { get; set; }

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
