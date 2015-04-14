using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLayer.Entities.Built
{
    public class BuiltToggleInterest
    {
        public ToggleResult result { get; set; }

    }

    public class ToggleResult
    {
        public ToggleApi api { get; set; }
    }

    public class ToggleApi
    {
        public ToggleResponse response { get; set; }
        public string user_key { get; set; }
        public string name { get; set; }
        public string session_id { get; set; }
        public string abbreviation { get; set; }
        public string title { get; set; }
        public string operation { get; set; }
    }

    public class ToggleResponse
    {
        public string _ { get; set; }
        public Toggle__invalid_type__ __invalid_name__ { get; set; }
    }

    public class Toggle__invalid_type__
    {
        public string code { get; set; }
    }
}
