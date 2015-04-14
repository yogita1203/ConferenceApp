using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLayer.Entities.Built
{
    public class BuiltSection
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [ForeignKey(typeof(BuiltMenu))]
        public int BuiltMenuId { get; set; }

        [ManyToOne]
        public BuiltMenu BuiltMenu { get; set; }

        public string uid { get; set; }
        public string sectionname { get; set; }
        public Int32 order { get; set; }

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<BuiltSectionItems> menuitems { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public string deleted_at { get; set; }
    }
}
