using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLayer.Entities.Built
{
    public class Dollar
    {
        public string code { get; set; }
    }

    public class Response
    {
        public string _ { get; set; }

        [JsonProperty("$")]
        public Dollar dollar { get; set; }
    }

    public class _Api
    {
        public Response response { get; set; }
    }

    public class _Result
    {
        public _Api api { get; set; }
    }

    public class AddRemoveSessionSuccess
    {
        public _Result result { get; set; }
    }

    /*=============================================================================*/
}
