using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLayer.Entities.Built
{
    public class BuiltSocialTwitter
    {
        public string uid { get; set; }
        public string hashtags { get; set; }
        public string posting_hashtags { get; set; }
        public string consumer_key { get; set; }
        public string consumer_secret { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public string deleted_at { get; set; }
    }
}
