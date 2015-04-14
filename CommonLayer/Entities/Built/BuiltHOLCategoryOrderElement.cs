using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLayer.Entities.Built
{
    public class BuiltHOLCategoryOrderElement
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [ForeignKey(typeof(BuiltHOLCategoryOrder))]
        public int BuiltHOLCategoryOrderId { get; set; }
        
        [ManyToOne]
        public BuiltHOLCategoryOrder BuiltHOLCategoryOrder { get; set; }
        public string uid { get; set; }
        public string hol_category_name { get; set; }
        public string hol_category_sequence { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public string deleted_at { get; set; }
    }
}
