using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLayer.Entities.Built
{
    public class AllSocialModel
    {
        public string content_text { get; set; }
        public string type { get; set; }
        public string user_uid { get; set; }
        public string uid { get; set; }
        public string username { get; set; }
        public string user_image_url { get; set; }
        public Int64 subtype { get; set; }
        public string media_url { get; set; }
        public string media_thumbnailURL { get; set; }

        public string media_height { get; set; }

        public string media_width { get; set; }

        public string social_object { get; set; }
        public string createdat { get; set; }
        public string actionLink { get; set; }

        //public object twitterData { get; set; }
        //@property (nonatomic, retain) id twitterData;
        public string commentsCount { get; set; }
        public string likesCount { get; set; }
        public Int64 id_as_index { get; set; }
    }
}
