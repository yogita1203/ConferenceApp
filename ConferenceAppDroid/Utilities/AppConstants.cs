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

namespace ConferenceAppDroid.Utilities
{
    public class AppConstants
    {
        public static String DEFAULT_TRACKS_COLOR = "242424";
        public static String MY_INTEREST_SPEAKER_FRAGMENT = "MY_INTEREST_SPEAKER_FRAGMENT";
        public static String SESSION_DETAIL_PAGER_ADAPTER = "SESSION_DETAIL_PAGER_ADAPTER";
        public static String SPEAKER_DETAIL_PAGER_ADAPTER = "SPEAKER_DETAIL_PAGER_ADAPTER";
        public static bool IS_SERVICE_WORKING = false;
        public static String MOBILE_LOGO = "Mobile Logo";
        public static bool SEARCH_SPONSOR_SHOW_HIDE = false;
        public static String SPONSOR_SELECTED_FROM_EXPLORE = "";
        public static String SPEAKER_SEARCH_TEXT = "";
        public static long TOTALTIME = 300;
        public static long INTERVAL = 150;
        public static String SPONSORS_BY_CONTRIBUTION = "SPONSORS_BY_CONTRIBUTION";
        public static String APPUTILITIES = "APPUTILITIES";
        public static int TEMP_POSITION = -1;

        public static String twitter_consumer_key = "uiWXOi5PlziQJ67Qszg70Ijuq";
        public static String twitter_consumer_secret = "Tsr4nAbsr8uaZhZn0Gqc48X9qhfBUnl0XztAPcQLkF2ksdu3AM";
        public static String twitter_access_token = "98090552-PnaWkBkAZGgV5vkLpWWwWMIEuouXzFZVFCB5Ssc4Y";
        public static String twitter_access_token_secret = "n2cYzxHY3pVLvSpAkygg4nx2cUGMViGtHQ4vQsxE2ZS5S";
        public static String TWITTER_CALLBACK_URL = "x-demo-twitter://demotwitterlogin";
        public static String IEXTRA_OAUTH_VERIFIER = "oauth_verifier";
        public static String SHARED_PREF_KEY_TOKEN = "demo_oauth_token";
        public static String SHARED_PREF_KEY_SECRET = "demo_oauth_token_secret";
        public static String SHARED_PREF_KEY_USER_NAME = "logged_in_user_name";
        public static String SHARED_PREF_KEY_USER_ID = "logged_in_user_id";
        public static String SHARED_PREF_KEY_USER_HANDLER = "logged_in_user_handler";
        public static String SHARED_PREF_KEY_USER_IMAGE = "logged_in_user_image";
    }
}