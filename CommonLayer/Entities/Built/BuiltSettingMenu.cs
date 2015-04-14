//using SQLite.Net.Attributes;
//using SQLiteNetExtensions.Attributes;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace CommonLayer.Entities.Built
//{
//    public class BuiltSettingMenu
//    {
//        [PrimaryKey, AutoIncrement]
//        public int Id { get; set; }
//        [OneToMany(CascadeOperations = CascadeOperation.All)]
//        public List<Menus> menus { get; set; }
//    }
//    public class Menus
//    {
//        [PrimaryKey, AutoIncrement]
//        public int Id { get; set; }
//        [ForeignKey(typeof(BuiltSettingMenu))]
//        public int BuiltSettingMenuId { get; set; }

//        [ManyToOne]
//        public BuiltSettingMenu BuiltSettingMenu { get; set; }
//        public Int32 order { get; set; }
//        public string section_name { get; set; }

//        [OneToMany(CascadeOperations = CascadeOperation.All)]
//        public List<Submenus> submenu { get; set; }
//    }

//    public class Submenus
//    {
//        [PrimaryKey, AutoIncrement]
//        public int Id { get; set; }
//        [ForeignKey(typeof(Menus))]
//        public int MenusId { get; set; }

//        [ManyToOne]
//        public Menus Menus { get; set; }
//        public Int32 order { get; set; }
//        public string name { get; set; }
//        public string link { get; set; }
//    }
//}
