using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLayer.Entities.Built
{
    public class BuiltMySession
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string session_time_id { get; set; }
    }

    public class UserInterestSession
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string session_time_id { get; set; }
    }
}
