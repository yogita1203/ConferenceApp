using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLayer.Entities.Built
{
    public class BuiltSFTransportation
    {
        //[PrimaryKey, AutoIncrement]
        public string uid { get; set; }
        public string android_link { get; set; }
        public string categery { get; set; }
        public object icon { get; set; }
        public string ios_link { get; set; }
        public string name { get; set; }
        public string short_desc { get; set; }
        public string app_store_link { get; set; }
        //public Int32 PRIMARY KEY(_id) { get;set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public string deleted_at { get; set; }
    }
}
