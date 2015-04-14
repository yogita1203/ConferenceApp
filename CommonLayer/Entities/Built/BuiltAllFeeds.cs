using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLayer.Entities.Built
{
    public class BuiltAllFeeds
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [ForeignKey(typeof(BuiltConfigSocial))]
        public int BuiltConfigSocialId { get; set; }
        [OneToOne]
        public BuiltConfigSocial BuiltConfigSocial { get; set; }
        public string uid { get; set; }
        public string tweet_text { get; set; }
        public string hashtags { get; set; }
        public string url { get; set; }
        public string email_subject { get; set; }
        public string photo_hashtags { get; set; }
        public string selfie_hashtags { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public string deleted_at { get; set; }
    }
}
