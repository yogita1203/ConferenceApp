using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLayer.Entities.Built
{
    public class BuiltApplicationUser
    {
        public string uid { get; set; }
        public object ACL { get; set; }
        public object __loc { get; set; }
        public string[] achievements { get; set; }
        public bool active { get; set; }
        public BuiltActivity activity { get; set; }
        public string address1 { get; set; }
        public string address2 { get; set; }
        public string address3 { get; set; }
        public string app_user_object_uid { get; set; }
        public string[] attendee_type { get; set; }
        public string badge_id { get; set; }
        public string badge_print_location { get; set; }
        public string cannot_view_or_schedule { get; set; }
        public string city { get; set; }
        public string company_name { get; set; }
        public string country_id { get; set; }
        public string create_date { get; set; }
        public string created_at { get; set; }
        public string device_type { get; set; }
        public string email { get; set; }
        public string email2 { get; set; }
        public string external_api_identifier { get; set; }
        public string fax_number { get; set; }
        public string first_name { get; set; }
        public string foreign_state { get; set; }
        public bool gaming_enabled { get; set; }
        public string job_title { get; set; }
        public string kellie_reporting_value { get; set; }
        public string last_modified_date { get; set; }
        public string last_name { get; set; }
        public string middle_initial { get; set; }
        public string paid { get; set; }
        public string phone2_number { get; set; }
        public string phone_number { get; set; }
        public string preferred_name { get; set; }
        public string private_view_private_schedule { get; set; }
        public string public_bio { get; set; }
        public string public_view_private_schedule { get; set; }
        public bool published { get; set; }
        public BuiltRegcodes regcodes { get; set; }
        public string registered { get; set; }
        public string report_classification { get; set; }
        public BuiltSchedule schedule { get; set; }
        public string screen_name { get; set; }
        public string state_id { get; set; }
        public string[] tags { get; set; }
        public string updated_at { get; set; }
        public string user_ref { get; set; }
        public string username { get; set; }
        public string zip { get; set; }
        public int _version { get; set; }
        public string deleted_at { get; set; }
    }
}
