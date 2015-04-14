using Newtonsoft.Json;
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLayer.Entities.Built
{
    public class BuiltSettingsMenu
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string uid { get; set; }

        [OneToMany(CascadeOperations = CascadeOperation.All), JsonProperty("menu")]
        public List<Menus> menu { get; set; }

        public string created_at { get; set; }
        public string updated_at { get; set; }
        public string deleted_at { get; set; }
    }
    public class Menus
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [ForeignKey(typeof(BuiltSettingsMenu))]
        public int BuiltSettingsMenuId { get; set; }

        [ManyToOne]
        public BuiltSettingsMenu BuiltSettingsMenu { get; set; }
        public Int32 order { get; set; }
        public string section_name { get; set; }

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<Submenus> sub_menu { get; set; }
    }

    public class Submenus
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [ForeignKey(typeof(Menus))]
        public int MenusId { get; set; }

        [ManyToOne]
        public Menus Menus { get; set; }
        public Int32 order { get; set; }
        public string name { get; set; }
        public string link { get; set; }

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<LinkGroup> link_group { get; set; }
    }

    public class LinkGroup
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [ForeignKey(typeof(Submenus))]
        public int SubmenusId { get; set; }

        [ManyToOne]
        public Submenus Submenus { get; set; }

        public string link { get; set; }
        public string technology { get; set; }
    }
}
