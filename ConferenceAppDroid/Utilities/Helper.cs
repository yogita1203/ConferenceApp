using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using CommonLayer.Entities.Built;
using System.Globalization;
using System.IO;
using CommonLayer;
using Android.Util;
using Android.Content;
using Android.Widget;
using Android.Graphics;
using BuiltSDK;
using Newtonsoft.Json.Linq;
using Android.App;
using Android.Views.InputMethods;


namespace  ConferenceAppDroid.Utilities
{
    public class Helper
    {
        public static bool isWebViewCalled = false;

        public const string DateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'sszzz";

        public const string SessionAddedString = "The session has been added to your schedule.";
        public const string SessionRemovedString = "The session has been removed from your schedule.";
        public const string InterestAddedString = "The session has been added to your interests.";
        public const string InterestRemovedString = "The session has been removed from your interests.";
        public const string CanntotUnschedule = "You cannot unschedule this session.";

        public static int dpToPx(Context context, int dp)
        {
            int px = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, dp, context.Resources.DisplayMetrics);
            return px;
        }

        public static float getCenterX(float parentWidth, float viewWidth)
        {
            return (parentWidth / 2) - (viewWidth / 2);
        }

        public static float getCenterY(float parentHeight, float viewHeight)
        {
            return (parentHeight / 2) - (viewHeight / 2);
        }

        internal static string getFoodDrinkLink(List<FDLinkGroup> group)
        {
            try
            {
                var ipad = group.FirstOrDefault(p => p.technology.ToLower() == "ipad");
                if (ipad != null)
                    return ipad.link;

                var ios = group.FirstOrDefault(p => p.technology.ToLower() == "ios");
                if (ios != null)
                    return ios.link;

                var all = group.FirstOrDefault(p => p.technology.ToLower() == "all");
                if (all != null)
                    return all.link;
            }
            catch
            { }
            return String.Empty; ;
        }

        internal static string getTransportationLink(List<TransLinkGroup> group)
        {
            try
            {
                var ipad = group.FirstOrDefault(p => p.technology.ToLower() == "ipad");
                if (ipad != null)
                    return ipad.link;

                var ios = group.FirstOrDefault(p => p.technology.ToLower() == "ios");
                if (ios != null)
                    return ios.link;

                var all = group.FirstOrDefault(p => p.technology.ToLower() == "all");
                if (all != null)
                    return all.link;
            }
            catch
            { }
            return String.Empty; ;
        }

        internal static string getSettingsLink(List<LinkGroup> group)
        {
            try
            {
                var ipad = group.FirstOrDefault(p => p.technology.ToLower() == "ipad");
                if (ipad != null)
                    return ipad.link;

                var ios = group.FirstOrDefault(p => p.technology.ToLower() == "ios");
                if (ios != null)
                    return ios.link;

                var all = group.FirstOrDefault(p => p.technology.ToLower() == "all");
                if (all != null)
                    return all.link;
            }
            catch
            { }
            return String.Empty; ;
        }

        public static String getStringToShowFragment(String link)
        {
            if (link.Contains("/"))
            {
                int index1 = link.LastIndexOf("/") + 1;
                if (index1 != -1)
                {
                    link = link.Substring(index1);
                    return link;
                }
                else
                {
                    return string.Empty;
                }
            }
            else
            {
                return null;
            }
        }

        public static void showProgress( Android.App.Activity activity,bool showProgress)
        {
            ProgressDialog progressDialog = new ProgressDialog(activity);
            if (showProgress)
            {
                progressDialog.SetMessage("Please wait…");
                progressDialog.Show();
            }
            else
            {
                progressDialog.Dismiss();
            }
        }

        public static void showAlertDialog(Android.App.Activity activity, string title, string message)
        {
            new Android.App.AlertDialog.Builder(activity).SetTitle(title).SetMessage(message)
                                           .SetPositiveButton("Yes", (sender, args) =>
                                           {
                                               Intent intent = new Intent(activity, typeof(LoginActivity));
                                               activity.StartActivity(intent);
                                           })
                                           .SetNegativeButton("No", (sender, args) => { }).Show();
        }

        internal static string GetErrorMessage(BuiltError error)
        {
            try
            {
                var errors = error.getErrors();
                var jarr = errors["error"] as JArray;
                return jarr.First.ToString();
            }
            catch
            {
                return error.Message.Contains("BuiltSDK") || error.Message.Length > 100 ? "Error occured" : error.Message;
            }
        }

        public static string convertToBuiltDate(string p)
        {
            if (String.IsNullOrWhiteSpace(p))
                return String.Empty;

            if (p.ToLower().Contains("days"))
            {
                var allDate = p;
                return allDate;
            }

            try
            {
                var date = DateTime.Parse(p);
                return Helper.ToDateTimeString(date, "yyyy-MM-dd");
            }
            catch
            {
                return String.Empty;
            }
        }


        public static string GetExhibitorImageUrl(List<BuiltExhibitorFile> exhibitor_files)
        {
            if (exhibitor_files != null && exhibitor_files.Count > 0)
            {
                var builtExhibitorFile = exhibitor_files.Where(p => p.type != null).FirstOrDefault(p => p.type.Equals("mobile logo", StringComparison.InvariantCultureIgnoreCase));
                if (builtExhibitorFile != null)
                    return builtExhibitorFile.url;
                else
                    return null;
            }
            else
            {
                return null;
            }
        }

        public static string ToDateTimeString(DateTime dt, string format)
        {
            if (String.IsNullOrWhiteSpace(format))
                return String.Empty;
            else
                return dt.ToString(format);
        }
        public static string convertToNewsDate(string dateTime)
        {
            if (String.IsNullOrWhiteSpace(dateTime))
                return String.Empty;
            try
            {
                var date = DateTime.Parse(dateTime);
                return Helper.ToDateTimeString(date, "MMMM dd, yyyy");
            }
            catch
            {
                return String.Empty;
            }
        }

        public static string getUnicodeString(Android.App.Activity activity, TextView textview, string strTemp)
        {
            Typeface custom_font = Typeface.CreateFromAsset(activity.Assets, "Fonts/FontAwesome.ttf");
            textview.Typeface = custom_font;
            var str = strTemp;
            var val = str.Substring(3).Replace(";", "");
            int code = int.Parse(val, System.Globalization.NumberStyles.HexNumber);
            string unicodeString = char.ConvertFromUtf32(code).ToString();
            return unicodeString;
        }

        public static string ConvertToTimeAgo(DateTime when)
        {
            var difference = (DateTime.Now - when).TotalSeconds;

            string whichFormat;
            int valueToFormat;
            if (difference < 2.0)
            {
                whichFormat = "Your data is up to date";
                valueToFormat = 0;
            }
            else if (when.Date == DateTime.Now.Date)
            {
                return "Last Updated: Today at " + when.ToString("hh:mm tt");
            }
            else if (when.Date == DateTime.Now.AddDays(-1).Date)
            {
                return "Last Updated: Yesterday at " + when.ToString("hh:mm tt");
            }
            else
            {
                whichFormat = "Last Updated: {0}d ago";
                valueToFormat = (int)(difference / (3600 * 24));
            }

            return string.Format(whichFormat, valueToFormat);
        }

        public static string SocialToTimeAgo(DateTime when)
        {
            var difference = (TimeZoneInfo.ConvertTime(DateTime.Now, DataManager.destinationTimeZone)-TimeZoneInfo.ConvertTime(when,DataManager.destinationTimeZone)).TotalSeconds;

            string whichFormat;
            int valueToFormat;
            
            if (difference < 60.0)
            {
                whichFormat = "{0}s";
                valueToFormat = (int)difference;
            }
            else if (difference < 3600.0)
            {
                whichFormat = "{0}m";
                valueToFormat = (int)(difference / 60);
            }
            else if (difference < 24 * 3600)
            {
                whichFormat = "{0}h";
                valueToFormat = (int)(difference / (3600));
            }
            else if (difference < 24 * 3600 * 7)
            {
                whichFormat = "{0}d";
                valueToFormat = (int)(difference / (3600 * 24));
            }
            else if (difference < 24 * 3600 * 30)
            {
                whichFormat = "{0}w";
                valueToFormat = (int)(difference / (3600 * 24 * 7));
            }
            else
            {
                whichFormat = "{0}M";
                valueToFormat = (int)(difference / (3600 * 24 * 30));
            }


            return string.Format(whichFormat, valueToFormat);
        }

        public static string ToDateString(DateTime input)
        {
            return input.ToString(DateTimeFormat);
        }

        public static DateTime FromDateString(string input)
        {
            try
            {
                CultureInfo provider = CultureInfo.InvariantCulture;
                if (input != null)
                {
                    return DateTime.ParseExact(input, DateTimeFormat, provider);
                }
                else
                {
                    return DateTime.Now;
                }
            }
            catch
            {
                return DateTime.Now;
            }
        }

        public static string convertToTodayTomorrowDate(string Date)
        {
            if (String.IsNullOrWhiteSpace(Date))
                return String.Empty;

            try
            {
                var date = DateTime.Parse(Date).Date;
                if (date.Day == DateTime.Now.Day && DateTime.Now.Month == date.Month && DateTime.Now.Year == date.Year)
                {
                    Date = "Today";
                    return Date;
                }
                else if (date.Day == DateTime.Now.Day + 1 && DateTime.Now.Month == date.Month && DateTime.Now.Year == date.Year)
                {
                    Date = "Tomorrow";
                    return Date;
                }
                else
                {
                    return ToDateTimeString(date, "ddd MMM dd");
                }
            }
            catch
            {
                return String.Empty;
            }
        }

        public static string convertToStartEndDate(string time, string length)
        {
            if (String.IsNullOrWhiteSpace(time))
                return String.Empty;

            try
            {
                string date = ToDateTimeString(DateTime.Parse(time), "hh:mm tt");
                string endDate = ToDateTimeString(DateTime.Parse(time).AddMinutes(Convert.ToDouble(length)), "hh:mm tt");

                string actualDate = string.Format("{0} - {1}", date, endDate);
                return actualDate;
            }
            catch
            {
                return String.Empty;
            }
        }

        public static int timeConverterForBuiltTimeString(string str)
        {
            if (String.IsNullOrWhiteSpace(str))
                return 0;

            try
            {
                str = str.Replace(":", String.Empty);
                return Convert.ToInt32(str);
            }
            catch
            {
                return 0;
            }
        }

        public static int timeConverterForCurrentHourMinute()
        {
            var dt = TimeZoneInfo.ConvertTime(DateTime.Now, DataManager.destinationTimeZone);
            string strNow = dt.ToString("HH") + dt.ToString("MM");
            return Convert.ToInt32(strNow);
        }


        public static string convertToDate(string Date)
        {
            if (String.IsNullOrWhiteSpace(Date))
                return String.Empty;
            var date = DateTime.Parse(Date).ToString("ddd, MMM dd");
            return date;
        }

        public static string convertToDateInterest(string Date)
        {
            if (String.IsNullOrEmpty(Date))
                return String.Empty;

            if (Date.ToLower().Contains("days"))
            {
                return Date;
            }
            if (Date.Contains("-"))
            {
                try
                {
                    var date = DateTime.Parse(Date);
                    return ToDateTimeString(date, "ddd, MMM dd");
                }
                catch
                {
                    return String.Empty;
                }
            }
            else
            {
                try
                {
                    CultureInfo provider = CultureInfo.InvariantCulture;
                    var format = "ddd MMM dd yyyy";
                    var date = DateTime.ParseExact(Date, format, provider).ToString("ddd, MMM dd");
                    return date;
                }
                catch
                {
                    return String.Empty;
                }
            }
        }

    }

    public enum Screens
    {
        Session,
        SpeakerSession,
        Agenda
    }
}