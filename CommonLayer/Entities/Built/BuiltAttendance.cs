using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLayer.Entities.Built
{
    public class BuiltAttendance
    {
        //[PrimaryKey, AutoIncrement]
        public string uid { get; set; }
        public string create_date { get; set; }
        public string sessiontime_id { get; set; }
        public string session_id { get; set; }
        public string user_ref { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public string deleted_at { get; set; }
        //public Int32 PRIMARY KEY(_id) { get;set; }
    }
}
