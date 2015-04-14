using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using CommonLayer.Entities.Built;

namespace ConferenceAppDroid.Utilities
{
    public class AppSettings
    {
        private static AppSettings _instance;
        public static string dbFileName = "vmworld_pex_uat.db";
        public int NewSurveyCount;
        public static AppSettings Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AppSettings();
                }
                return _instance;
            }
            
        }
        private List<string> _MySessionIds;

        public List<string> MySessionIds
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
        public List<BuiltTracks> AllTracks;
        public Dictionary<string, BuiltTracks[]> TrackDictionary = new Dictionary<string, BuiltTracks[]>();
        public BuiltConfig config;
        public ExtensionApplicationUser ApplicationUser;
        public List<BuiltSessionTime> FeaturedSessions;
        public List<BuiltSpeaker> speakerScreen;
        public List<BuiltSessionSpeaker> SessionDetailSpeaker;

        public void setTwitterAccessTokenAndSecret(Context context, String accessToken, String tokenSecret)
        {
            ISharedPreferences sharedPrefs = Prefs.get(context);
            ISharedPreferencesEditor editor = sharedPrefs.Edit();
            editor.PutString(AppConstants.SHARED_PREF_KEY_TOKEN, accessToken);
            editor.PutString(AppConstants.SHARED_PREF_KEY_SECRET, tokenSecret);
            editor.Commit();
        }

        public void setTwitterUserName(Context context, String userName)
        {
            ISharedPreferences sharedPrefs = Prefs.get(context);
            ISharedPreferencesEditor editor = sharedPrefs.Edit();
            editor.PutString(AppConstants.SHARED_PREF_KEY_USER_NAME, userName);
            editor.Commit();
        }
        public String getTwitterUserName(Context context)
        {
            ISharedPreferences sharedPrefs = Prefs.get(context);
            return sharedPrefs.GetString(AppConstants.SHARED_PREF_KEY_USER_NAME, null);
        }
        public String getTwitterUserHandler(Context context)
        {
            ISharedPreferences sharedPrefs = Prefs.get(context);
            return sharedPrefs.GetString(AppConstants.SHARED_PREF_KEY_USER_HANDLER, null);
        }

        public void setTwitterUserId(Context context, long userID)
        {
            ISharedPreferences sharedPrefs = Prefs.get(context);
            ISharedPreferencesEditor editor = sharedPrefs.Edit();
            editor.PutLong(AppConstants.SHARED_PREF_KEY_USER_ID, userID);
            editor.Commit();
        }
        public void setTwitterUserHandler(Context context, String userHandler)
        {
            ISharedPreferences sharedPrefs = Prefs.get(context);
            ISharedPreferencesEditor editor = sharedPrefs.Edit();
            editor.PutString(AppConstants.SHARED_PREF_KEY_USER_HANDLER, userHandler);
            editor.Commit();
        }
        public void setTwitterUserImage(Context context, String userImage)
        {
            ISharedPreferences sharedPrefs = Prefs.get(context);
            ISharedPreferencesEditor editor = sharedPrefs.Edit();
            editor.PutString(AppConstants.SHARED_PREF_KEY_USER_IMAGE, userImage);
            editor.Commit();
        }

        public String getTwitterAccessToken(Context context)
        {
            ISharedPreferences sharedPrefs = Prefs.get(context);
            return sharedPrefs.GetString(AppConstants.SHARED_PREF_KEY_TOKEN, null);
        }

        public String getTwitterAccessTokenSecret(Context context)
        {
            ISharedPreferences sharedPrefs = Prefs.get(context);
            return sharedPrefs.GetString(AppConstants.SHARED_PREF_KEY_SECRET, null);
        }

        public bool isTwitterAuthenticationDone(Context context)
        {
            ISharedPreferences sharedPrefs = Prefs.get(context);
            return sharedPrefs.GetString(AppConstants.SHARED_PREF_KEY_TOKEN, null) != null;
        }

        public string GeneralSession = "General Session";
        public string ConferenceFunction = "Conference Function";
    }
}