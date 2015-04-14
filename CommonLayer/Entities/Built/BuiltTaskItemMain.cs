using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace VMWorldEvent.Services.Entities.Sqlite
{
    public class BuiltTaskItemMain
    {
            public bool published { get; set; }
            public List<object> tags { get; set; }
            public string app_user_object_uid { get; set; }
            public string uid { get; set; }
            public string category { get; set; }
            public List<string> owner { get; set; }
            public List<string> shared_with { get; set; }
            //public object shared_with { get; set; }
            public string updated_at { get; set; }
            public string created_at { get; set; }
            public Dictionary<string, object> UPSERT { get; set; }
            public string deleted_at { get; set; }
            //public ACL ACL { get; set; }
    }

}
