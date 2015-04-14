using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLayer.Entities.Built
{
    public class BuiltLeaderBoardSession
    {
        public string uid { get; set; }
        public int? points { get; set; }
        public List<object> claimed_sessions { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public string deleted_at { get; set; }
    }

    public class BuiltLeaderBoardActivity
    {
        public string uid { get; set; }
        public int total_points { get; set; }
        public BuiltQr qr { get; set; }
        public BuiltSurvey survey { get; set; }
        public BuiltLeaderBoardSession session { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public string deleted_at { get; set; }
    }

    public class BuiltLeaderBoardResult
    {
        public string uid { get; set; }
        public BuiltLeaderBoardActivity activity { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public string deleted_at { get; set; }
    }

    public class BuiltLeaderBoardRootObject
    {
        public string uid { get; set; }
        public List<BuiltLeaderBoardResult> result { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public string deleted_at { get; set; }
    }
}
