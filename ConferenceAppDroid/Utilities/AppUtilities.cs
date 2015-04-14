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
using Android.Net;
using ConferenceAppDroid.Activities;
using Android.Support.V4.App;
using ConferenceAppDroid.Fragments;
using ConferenceAppDroid.Interfaces;

namespace ConferenceAppDroid.Utilities
{
    public class AppUtilities
    {
        public static bool IsNetworkAvailable(Context context)
        {

            ConnectivityManager connectivityManager = (ConnectivityManager)context.GetSystemService(Context.ConnectivityService);
            NetworkInfo netInfo = connectivityManager.ActiveNetworkInfo;
            if (connectivityManager != null && netInfo != null)
            {

                if (netInfo.IsConnectedOrConnecting)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public void isFromPauseResume(Activity activity, bool isFromPause)
        {
            if (isFromPause)
            {
                ((MainApplication)activity.Application).startActivityTransitionTimer();
            }
            else
            {
                MainApplication myApp = null;
                try
                {
                    myApp = (MainApplication)activity.Application;
                }
                catch (Exception e)
                {
 
                }

                if (myApp.wasInBackground)
                {
                    if (AppUtilities.IsNetworkAvailable(activity.ApplicationContext) && !AppConstants.IS_SERVICE_WORKING)
                    {
                        Intent intent = new Intent(BroadcastReceivers.BackgroundUpdatesStartReceiver.action);
                        activity.SendBroadcast(intent);
                    }
                }
                myApp.stopActivityTransitionTimer();

            }
        }

        public static string UppercaseFirst(string s)
        {
            // Check for empty string.
            if (string.IsNullOrWhiteSpace(s))
            {
                return string.Empty;
            }
            // Return char and concat substring.
            return char.ToUpper(s[0]) + s.Substring(1);
        }


         public static void postOnTwitter(Context context, String message, String imageUrl, IGetTweetStatusObject getTweetStatusObject) 
         {
        if (IsNetworkAvailable(context))
        {
            if (AppSettings.Instance.isTwitterAuthenticationDone(context)) 
            {
                SocialTweetDialogFragment tweetDialog = new SocialTweetDialogFragment();

                Bundle bundle = new Bundle();
                bundle.PutString("postMessage", message);
                if (imageUrl != null) 
                {
                    bundle.PutString("imageUrl", imageUrl);
                }
                tweetDialog.Arguments=(bundle);

                Android.Support.V4.App.FragmentTransaction transaction = ((FragmentActivity) context).SupportFragmentManager.BeginTransaction();
                transaction.Add(tweetDialog, "loading");
                transaction.CommitAllowingStateLoss();

            }
            else
            {
                //			mContext.startActivity(new Intent(mContext, TwitterLoginActivity.class));
                Intent twitterLoginIntent = new Intent(context, typeof(TwitterLoginActivity));
                Bundle bundle = new Bundle();
                bundle.PutString("postMessage", message);
                if (imageUrl != null) {
                    bundle.PutString("imageUrl", imageUrl);
                }
                twitterLoginIntent.PutExtras(bundle);
                context.StartActivity(twitterLoginIntent);
            }
        } else {
            Toast.MakeText(context, context.Resources.GetString(Resource.String.no_network_text), ToastLength.Short).Show();
        }
    }
    }
}