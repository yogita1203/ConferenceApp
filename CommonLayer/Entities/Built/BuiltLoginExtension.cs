using CommonLayer.Entities.Built;
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLayer.Entities.Built
{
    public class BuiltLoginExtensionJson
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        
        public string login_json { get; set; }
    }

    public class BuiltLoginExtension
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.All)]
        public ExtensionApplicationUser application_user { get; set; }
    }
    public class ExtensionApplicationUser
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [ForeignKey(typeof(BuiltLoginExtension))]
        public int BuiltLoginExtensionId { get; set; }
        
        [OneToOne]
        public BuiltLoginExtension BuiltLoginExtension { get; set; }

        public string updated_at { get; set; }
        public string created_at { get; set; }
        public string user_ref { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string email { get; set; }
        public string public_view_private_schedule { get; set; }
        public string private_view_private_schedule { get; set; }
        public string cannot_view_or_schedule { get; set; }
        [OneToOne(CascadeOperations = CascadeOperation.All)]
        public Schedule schedule { get; set; }

        [OneToMany(CascadeOperations= CascadeOperation.All)]
        public List<SurveysTaken> surveys_taken { get; set; }

        [Ignore]
        public List<string> attendee_type { get; set; }
        public string attendee_type_separated { get; set; }
        public string external_api_identifier { get; set; }
        public string device_type { get; set; }
        public bool active { get; set; }
        [Ignore]
        public List<string> tags { get; set; }
        public string Ext_tags { get; set; }
        public string app_user_object_uid { get; set; }
        public bool published { get; set; }
        public string uid { get; set; }
        public bool gaming_enabled { get; set; }
        [Ignore]
        public List<string> achievements { get; set; }
        public string Ext_achievements { get; set; }
        
        [OneToOne(CascadeOperations = CascadeOperation.All)]
        public Activity activity { get; set; }

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<ClaimedQr> claimed_qr { get; set; }
        public string username { get; set; }
        public int _version { get; set; }
        public string authtoken { get; set; }
    }

    public class SurveysTaken
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [ForeignKey(typeof(ExtensionApplicationUser))]
        public int ExtensionApplicationUserID { get; set; }

        [ManyToOne]
        public ExtensionApplicationUser ExtensionApplicationUser { get; set; }

        public string survey_id { get; set; }
        public string type { get; set; }
        public string survey_name { get; set; }
    }


    public class Schedule
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        
        [ForeignKey(typeof(ExtensionApplicationUser))]
        public int ExtensionApplicationUserId { get; set; }

        [OneToOne]
        public ExtensionApplicationUser ExtensionApplicationUser { get; set; }
        [Ignore]
        public List<string> session_time { get; set; }
        public string Ext_session_time { get; set; }

    }


    public class Session
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [ForeignKey(typeof(Activity))]
        public int ActivityId { get; set; }

        [OneToOne]
        public Activity Activity { get; set; }

        [Ignore]
        public List<string> claimed_sessions { get; set; }

        public string Ext_claimed_sessions { get; set; }
        public Int32 points { get; set; }
    }

    public class Survey
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [ForeignKey(typeof(Activity))]
        public int ActivityId { get; set; }

        [OneToOne]
        public Activity Activity { get; set; }

        [Ignore]
        public List<string> claimed_survey { get; set; }

        public string Ext_claimed_survey { get; set; }
        public Int32 points { get; set; }
    }

    public class Qr
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [ForeignKey(typeof(Activity))]
        public int ActivityId { get; set; }

        [OneToOne]
        public Activity Activity { get; set; }

        public Int32 points { get; set; }

        [Ignore]
        public List<string> claimed_qr { get; set; }

        public string Ext_claimed_qr { get; set; }
    }

    public class Activity
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [ForeignKey(typeof(ExtensionApplicationUser))]
        public int ExtensionApplicationUserId { get; set; }

        [OneToOne]
        public ExtensionApplicationUser ExtensionApplicationUser { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.All)]
        public Session session { get; set; }
        [OneToOne(CascadeOperations = CascadeOperation.All)]
        public Survey survey { get; set; }
        public int total_points { get; set; }
        [OneToOne(CascadeOperations = CascadeOperation.All)]
        public Qr qr { get; set; }
    }

    public class ClaimedQr
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [ForeignKey(typeof(ExtensionApplicationUser))]
        public int ExtensionApplicationUserId { get; set; }

        [ManyToOne]
        public ExtensionApplicationUser ExtensionApplicationUser { get; set; }

        public string date { get; set; }
        public string qr_code { get; set; }
        public int points { get; set; }
        public string achievement { get; set; }
    }
}
