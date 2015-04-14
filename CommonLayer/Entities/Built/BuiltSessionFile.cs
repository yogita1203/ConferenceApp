using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLayer.Entities.Built
{
    public class BuiltSessionFile
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [ForeignKey(typeof(BuiltSession))]
        public int BuiltSessionId { get; set; }

        [ManyToOne]      // Many to one relationship with Stock
        public BuiltSession BuiltSession { get; set; }

        //public string uid { get; set; }
        //public string filename { get; set; }
        public string url { get; set; }
        //public string size { get; set; }
        //public string created_at { get; set; }
        //public string updated_at { get; set; }
        //public string deleted_at { get; set; }
    }
}
