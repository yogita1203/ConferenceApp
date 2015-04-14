using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLayer.Entities.Built
{
    public class BuiltSectionItems
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [ForeignKey(typeof(BuiltSection))]
        public int BuiltSectionId { get; set; }

        [ManyToOne]
        public BuiltSection BuiltSection { get; set; }
        public string uid { get; set; }
        public string menuname { get; set; }
        public string descr { get; set; }
        public Int32 order { get; set; }
        public string link { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public string deleted_at { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.All)]
        public MenuIconFile menu_icon { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.All)]
        public MenuIconSelectedFile menu_icon_selected { get; set; }
        public string icon_name { get; set; }
        public string icon_code { get; set; }
    }

    public class MenuIconFile
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [ForeignKey(typeof(BuiltSectionItems))]
        public int BuiltSectionItemsId { get; set; }

        [OneToOne]
        public BuiltSectionItems BuiltSectionItems { get; set; }

        public string url { get; set; }
    }

    public class MenuIconSelectedFile
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [ForeignKey(typeof(BuiltSectionItems))]
        public int BuiltSectionItemsId { get; set; }

        [OneToOne]
        public BuiltSectionItems BuiltSectionItems { get; set; }

        public string url { get; set; }
    }
}
