using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLayer.Entities.Built
{
    public class BuiltImportantLinks
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string uid { get; set; }
        public string category { get; set; }
        public string desc{ get; set; }
        [OneToOne(CascadeOperations=CascadeOperation.All)]
        public BuiltImpLink link { get; set; }
        public string sequence { get; set; }
        public string title { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public string deleted_at { get; set; }
    }

    public class BuiltImpLink
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [ForeignKey(typeof(BuiltImportantLinks))]
        public int BuiltImportantLinksId { get; set; }

        public string href { get; set; }
        public string title { get; set; }
    }
}
