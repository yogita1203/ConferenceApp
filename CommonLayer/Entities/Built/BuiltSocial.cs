using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLayer.Entities.Built
{
    public class BuiltSocial
    {
        public string uid { get; set; }
        public List<BuiltSocialTwitter> twitter { get; set; }
        public List<BuiltSocialFacebook> facebook { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public string deleted_at { get; set; }
    }
}
