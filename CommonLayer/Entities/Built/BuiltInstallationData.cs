using BuiltSDK;
using System.Collections.Generic;

namespace CommonLayer.Entities.Built
{
    public class BuiltInstallationData
    {
        public string updated_at { get; set; }
        public string created_at { get; set; }
        public string device_type { get; set; }
        public string device_token { get; set; }
        public List<string> subscribed_to_channels { get; set; }
        public int badge { get; set; }
        public bool disable { get; set; }
        public string timezone { get; set; }
        public BuiltACL ACL { get; set; }
        public List<string> tags { get; set; }
        public string app_user_object_uid { get; set; }
        public bool published { get; set; }
        public string uid { get; set; }
        public object __loc { get; set; }
        public int _version { get; set; }

        public Certificate certificate { get; set; }
    }

    public class Certificate
    {
        public string environment { get; set; }
        public string name { get; set; }
    }
}
