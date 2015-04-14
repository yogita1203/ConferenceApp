using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLayer.Entities.Built
{
    public class BuiltOrderingInfo
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [ForeignKey(typeof(BuiltSponsors))]
        public int BuiltSponsorsId { get; set; }

        [ManyToOne]
        public BuiltSponsors BuiltSponsors { get; set; }

        public string uid { get; set; }
        public string type { get; set; }
        public Int32 order { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public string deleted_at { get; set; }
        public bool is_hidden { get; set; }
    }
}
