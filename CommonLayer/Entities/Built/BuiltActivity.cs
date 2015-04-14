using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLayer.Entities.Built
{
    public class BuiltActivity
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string uid { get; set; }
        public string _id { get; set; }
        public string type { get; set; }
        public Int32 points { get; set; }
        public Int32 max_point { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public string deleted_at { get; set; }
    }
}
