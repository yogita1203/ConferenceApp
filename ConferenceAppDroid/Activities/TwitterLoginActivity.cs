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
using Android.Support.V4.App;
using Android.Webkit;
using Twitter4j.Auth;
using ConferenceAppDroid.Utilities;
using Android.Graphics;
using Twitter4j;
using System.Threading;
using System.Threading.Tasks;

namespace ConferenceAppDroid.Activities
{
    [Activity(Label = "TwitterLoginActivity")]
    public class TwitterLoginActivity : FragmentActivity
    {

        public static int TWITTER_LOGIN_RESULT_CODE_SUCCESS = 1111;
        public static int TWITTER_LOGIN_RESULT_CODE_FAILURE = 2222;

        private static String TAG = "TwitterLoginActivity";

        private WebView twitterLoginWebView;
        private AlertDialog mAlertBuilder;
        private static String twitterConsumerKey;
        private static String twitterConsumerSecret;
        private static ITwitter twitter;
        private static RequestToken requestToken;

        private View actionBarView;
        private Bundle twitterActionBundle;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Create your application here
            setActionBar();

            twitterConsumerKey = AppConstants.twitter_consumer_key;
            twitterConsumerSecret = AppConstants.twitter_consumer_secret;

            twitter = TwitterHelper.getTwitterInstance(this);

            if (twitterConsumerKey == null || twitterConsumerSecret == null)
            {
                Intent intent = new Intent();
                intent.PutExtra("TWITTER_LOGIN_RESULT_CODE_FAILURE", TWITTER_LOGIN_RESULT_CODE_FAILURE);
                SetResult(Result.Ok, intent);
                Finish();
            }

            if (Intent != null)
            {

                twitterActionBundle = Intent.Extras;
            }

            mAlertBuilder = new AlertDialog.Builder(this).Create();
            mAlertBuilder.SetCancelable(true);
            mAlertBuilder.SetTitle(Resource.String.please_wait_title);
            View view = LayoutInflater.Inflate(Resource.Layout.view_loading, null);
            //((TextView) view.FindViewById(Resource.Id.messageTextViewFromLoading)).setTypeface(Typeface.createFromAsset(getAssets(), AppConstants.FONT_MEDIUM));
            ((TextView)view.FindViewById(Resource.Id.messageTextViewFromLoading)).Text = (GetString(Resource.String.authenticating_your_app_message));
            mAlertBuilder.SetView(view);
            mAlertBuilder.Show();

            mAlertBuilder.CancelEvent += (s, e) =>
            {
                mAlertBuilder = null;

                Finish();
            };

            twitterLoginWebView = (WebView)FindViewById(Resource.Id.twitterLoginWebView);
            twitterLoginWebView.SetBackgroundColor(Color.Transparent);
            twitterLoginWebView.SetLayerType(LayerType.Software, null);
            twitterLoginWebView.ClearCache(true);

            
            try
            {
                askOAuth().ContinueWith(t =>
                    {
                        twitterLoginWebView.SetWebViewClient(new CustomWebViewClient(twitter, requestToken, this, mAlertBuilder));
                    });
            }
            catch (Exception e)
            {
            }

        }

        private Task askOAuth()
        {
            twitter = TwitterHelper.getTwitterInstance(this);
           return Task.Run(() =>
            {
                try
                {
                    requestToken = twitter.GetOAuthRequestToken(AppConstants.TWITTER_CALLBACK_URL);
                }
                catch (Exception e)
                {
                    String errorString = e.ToString();
                    RunOnUiThread(() =>
                        {
                            if (mAlertBuilder != null)
                            {
                                mAlertBuilder.Cancel();
                            }
                            Toast.MakeText(this, errorString.ToString(), ToastLength.Short).Show();
                            Finish();
                        });
                    return;
                }
                this.RunOnUiThread(() => 
                {
                    Dictionary<String, String> noCacheHeaders = new Dictionary<String, String>(2);
                    noCacheHeaders.Add("Pragma", "no-cache");
                    noCacheHeaders.Add("Cache-Control", "no-cache");
                    twitterLoginWebView.LoadUrl(requestToken.AuthorizationURL, noCacheHeaders);
                });


            });
        }


        private void setActionBar()
        {

            actionBarView = LayoutInflater.Inflate(Resource.Layout.view_actionbar, null);
            ActionBar.CustomView = (actionBarView);
            SetContentView(Resource.Layout.activity_twitter_login);
            ActionBar.DisplayOptions = (ActionBarDisplayOptions.ShowCustom);

            actionBarView.FindViewById(Resource.Id.left_menu_btn).Visibility = ViewStates.Gone;
            actionBarView.FindViewById(Resource.Id.right_menu_btn).Visibility = ViewStates.Invisible;
            ((ImageView)actionBarView.FindViewById(Resource.Id.bottomImageView)).SetImageDrawable(Resources.GetDrawable(Resource.Drawable.close_icon_selector));
            actionBarView.FindViewById(Resource.Id.bottomImageView).Visibility = ViewStates.Visible;
            var bottomImageView = actionBarView.FindViewById(Resource.Id.bottomImageView);
            bottomImageView.Click += (s, e) =>
                {
                    Finish();
                };

            ((TextView)actionBarView.FindViewById(Resource.Id.titleTextView)).Text = (Resources.GetString(Resource.String.twitter_login_text).ToUpper());
            //((TextView) actionBarView.FindViewById(Resource.Id.titleTextView)).setTypeface(Typeface.createFromAsset(TwitterLoginActivity.this.getAssets(), AppConstants.FONT));
        }
    }

    public class CustomWebViewClient : WebViewClient
    {
        ITwitter twitter;
        RequestToken requestToken;
        Android.App.Activity activity;
        AlertDialog alertDialog;
        public CustomWebViewClient(ITwitter twitter, RequestToken requestToken, Android.App.Activity activity, AlertDialog alertDialog)
        {
            this.twitter = twitter;
            this.requestToken = requestToken;
            this.activity = activity;
            this.alertDialog = alertDialog;
        }

        public override bool ShouldOverrideUrlLoading(WebView view, string url)
        {
            if (url.Contains(AppConstants.TWITTER_CALLBACK_URL))
            {
                Android.Net.Uri uri = Android.Net.Uri.Parse(url);
                saveAccessTokenAndFinish(uri);
                return true;
            }
            return false;
        }

        public override void OnPageFinished(WebView view, string url)
        {
            base.OnPageFinished(view, url);

            if (alertDialog != null)
            {
                alertDialog.Dismiss();
            }
        }

        public override void OnPageStarted(WebView view, string url, Bitmap favicon)
        {
            base.OnPageStarted(view, url, favicon);

            if (alertDialog != null)
            {
                alertDialog.Show();
            }
        }

        private void saveAccessTokenAndFinish(Android.Net.Uri uri)
        {
            ThreadPool.QueueUserWorkItem(s =>
            {
                String verifier = uri.GetQueryParameter(AppConstants.IEXTRA_OAUTH_VERIFIER);
                try
                {
                    AccessToken accessToken = twitter.GetOAuthAccessToken(requestToken, verifier);
                    AppSettings.Instance.setTwitterAccessTokenAndSecret(activity, accessToken.Token, accessToken.TokenSecret);
                    Intent intent = new Intent();
                    intent.PutExtra("TWITTER_LOGIN_RESULT_CODE_SUCCESS", TwitterLoginActivity.TWITTER_LOGIN_RESULT_CODE_SUCCESS);
                    activity.SetResult(Result.Ok,intent);


                    long userID = accessToken.UserId;
                    IUser user = twitter.ShowUser(userID);
                    AppSettings.Instance.setTwitterUserId(activity.ApplicationContext, accessToken.UserId);
                    AppSettings.Instance.setTwitterUserName(activity.ApplicationContext, user.Name);
                    AppSettings.Instance.setTwitterUserHandler(activity.ApplicationContext, user.ScreenName);
                    AppSettings.Instance.setTwitterUserImage(activity.ApplicationContext, user.ProfileImageURL);

                }
                catch (Exception e)
                {
                    Intent intent = new Intent();
                    intent.PutExtra("TWITTER_LOGIN_RESULT_CODE_FAILURE", TwitterLoginActivity.TWITTER_LOGIN_RESULT_CODE_FAILURE);
                    activity.SetResult(Result.Ok, intent);
                }
               
                activity.Finish();
            });
        }
    }
}