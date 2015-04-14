using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using CommonLayer.Entities.Built;
using CommonLayer;

namespace ConferenceAppiOS
{
    public class AppSettings
    {
        public static string dbFileName = "vmworld_pex_uat.db";
        public const string tempDBName = "temp.db";
        public const string SurveyCountKey = "SurveyCount";
        public const string DeviceTokenKey = "DeviceToken";
        public const string IntroBgKey = "IntroBg";
        public const string GeneralSession = "General Session";
        public const string ConferenceFunction = "Conference Function";
        public static string SyncState = TitleHeaderView.upToDate;
        public static int NewSurveyCount;
        public static ExtensionApplicationUser ApplicationUser;
        public static NSString WebViewImageString;
        public static List<BuiltIntro> BuiltIntroList;
        public static List<BuiltSessionTime> FeaturedSessions;

        private static List<string> _MySessionIds;
        public static List<string> MySessionIds
        {
            get
            {
                if (_MySessionIds == null)
                {
                    _MySessionIds = new List<string>();
                }
                return _MySessionIds;

            }
            set
            {
                _MySessionIds = value;
            }
        }

        public static List<BuiltTracks> AllTracks;

        public const string RemoveSessionAlertMessage = "I confirm that I am removing this session from my schedule.";
        public const string venue_name = "Moscone Center";

        public static bool OpenNewNote;
        //public static bool IsLoggedIn()
        //{
        //    var userData = DataManager.GetCurrentUser(AppDelegate.Connection).Result;
        //    if (userData != null)
        //    {
        //        ApplicationUser = userData.application_user;
        //        return true;
        //    }
        //    return false;
        //}

		public static bool IsGaming()
		{
			return false;
		}

		public static bool IsSurvey()
		{
			return false;
		}
    }
}