using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//using MonoTouch.Foundation;
//using MonoTouch.UIKit;
using CommonLayer.Entities;
using BuiltSDK;

namespace CommonLayer.Entities.Built
{
    public class BuiltAppUserSession
    {
        public string uid { get; set; }
        public string[] claimed_sessions { get; set; }
        public int points { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public string deleted_at { get; set; }
    }
    public class BuiltQr
    {
        public string uid { get; set; }
        public string[] claimed_qr { get; set; }
        public int points { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public string deleted_at { get; set; }
    }

    public class BuiltSchedule
    {
        public string uid { get; set; }
        public List<string> session_time { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public string deleted_at { get; set; }
    }

    public class BuiltAppUserActivity
    {
        public string uid { get; set; }
        public BuiltAppUserSession session { get; set; }
        public BuiltQr qr { get; set; }
        public BuiltSurvey survey { get; set; }
        public int total_points { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public string deleted_at { get; set; }
    }

    public class BuiltSurvey
    {
        public string uid { get; set; }
        public List<string> claimed_survey { get; set; }
        public int points { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public string deleted_at { get; set; }
    }

    public class BuiltAppUser
    {
        public BuiltACL ACL { get; set; }
        public object __loc { get; set; }
        public List<string> achievements { get; set; }
        public bool active { get; set; }
        public BuiltAppUserActivity activity { get; set; }
        public string app_user_object_uid { get; set; }
        public List<string> attendee_type { get; set; }
        public string cannot_view_or_schedule { get; set; }
        public string created_at { get; set; }
        public string device_type { get; set; }
        public string external_api_identifier { get; set; }
        public string first_name { get; set; }
        public bool gaming_enabled { get; set; }
        public string last_name { get; set; }
        public string private_view_private_schedule { get; set; }
        public string public_view_private_schedule { get; set; }
        public bool published { get; set; }
        public BuiltSchedule schedule { get; set; }
        public List<object> tags { get; set; }
        public string uid { get; set; }
        public string updated_at { get; set; }
        public string username { get; set; }
        public int _version { get; set; }
        public string authtoken { get; set; }
        public string deleted_at { get; set; }
    }
}