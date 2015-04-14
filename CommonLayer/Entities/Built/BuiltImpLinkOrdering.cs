using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLayer.Entities.Built
{
    public class BuiltImpLinkOrdering
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }


        [ForeignKey(typeof(BuiltConfigImportantLinks))]
        public int BuiltConfigImportantLinksId { get; set; }

        [ManyToOne]
        public BuiltConfigImportantLinks BuiltConfigImportantLinks { get; set; }

        public string uid { get; set; }
        public int link_sequence { get; set; }
        public string link_category { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public string deleted_at { get; set; }
    }
}
