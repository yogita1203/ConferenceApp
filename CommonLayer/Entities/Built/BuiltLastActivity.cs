using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLayer.Entities.Built
{
    public class BuiltLastActivity
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string uid { get; set; }
        public string last_activity { get; set; }
    }
}
